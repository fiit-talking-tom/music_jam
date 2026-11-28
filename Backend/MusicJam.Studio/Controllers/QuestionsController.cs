using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicJam.Studio.Constants;
using MusicJam.Studio.Extensions;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Models.Tracks;
using MusicJam.Studio.Requests.Questions;
using MusicJam.Studio.Responses.Questions;
using MusicJam.Studio.Services.QuestionsService;
using Shared.Auth;
using Shared.Errors;

namespace MusicJam.Studio.Controllers;

[ApiController]
[Route("api/questions")]
[Authorize(AuthenticationSchemes = AuthConstants.DefaultAuthorizationScheme)]
public class QuestionsController(IQuestionsService questionsService, IApiErrorFactory apiErrorFactory) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QuestionCardResponse[]>> GetQuestions([FromQuery] GetQuestionsRequest request)
    {
        var userId = User.GetUserId();

        var result = await questionsService.GetQuestions(userId, request);
        if (result.TryGetFault(out var error, out var questions))
        {
            return ErrorResponse(error);
        }

        return questions;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Question), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateQuestion([FromForm] CreateQuestionRequest request)
    {
        var userId = User.GetUserId();

        var result = await questionsService.Create(userId, request);
        if (result.TryGetFault(out var error, out var question))
        {
            return ErrorResponse(error);
        }

        return CreatedAtAction(
            nameof(GetQuestion),
            new { questionId = question.Id },
            question);
    }

    [HttpDelete("{questionId:guid}")]
    [ProducesResponseType(typeof(Question), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteQuestion(Guid questionId)
    {
        var userId = User.GetUserId();

        var result = await questionsService.Delete(userId, questionId);
        if (result.TryGetFault(out var error, out _))
        {
            return ErrorResponse(error);
        }

        return NoContent();
    }

    [HttpPatch("{questionId:guid}")]
    [ProducesResponseType(typeof(Question), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PatchQuestion(Guid questionId, [FromForm] UpdateQuestionRequest request)
    {
        var userId = User.GetUserId();

        var result = await questionsService.Update(userId, questionId, PatchMode.PatchCopy, request);
        if (result.TryGetFault(out var error, out _))
        {
            return ErrorResponse(error);
        }

        return NoContent();
    }

    [HttpGet("{questionId:guid}")]
    [ProducesResponseType(typeof(Question), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQuestion(Guid questionId)
    {
        var userId = User.GetUserId();

        var result = await questionsService.Get(userId, questionId);
        if (result.TryGetFault(out var error, out var question))
        {
            return ErrorResponse(error);
        }

        return Ok(question);
    }

    [HttpGet("{questionId:guid}/answer-track")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task GetAnswerTrack(Guid questionId) => await GetTrackByType(questionId, TrackType.Answer);

    [HttpGet("{questionId:guid}/question-track")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task GetQuestionTrack(Guid questionId) => await GetTrackByType(questionId, TrackType.Question);

    private async Task GetTrackByType(Guid questionId, TrackType trackType)
    {
        var range = Request.GetTypedHeaders().Range?.Ranges.SingleOrDefault();

        var result = await questionsService.GetTrack(
            User.GetUserId(),
            questionId,
            trackType,
            range?.From,
            range?.To);

        var track = result.GetValueOrThrow();

        Response.StatusCode = range is null
            ? StatusCodes.Status200OK
            : StatusCodes.Status206PartialContent;

        Response.ContentType = track.ContentType;
        Response.ContentLength = track.ContentLength;

        Response.Headers.AcceptRanges = "bytes";

        if (range is not null)
        {
            Response.Headers.ContentRange = $"bytes {track.Start}-{track.End}/{track.TotalLength}";
        }

        await using (track.Stream)
        {
            await track.Stream.CopyToAsync(Response.Body);
        }
    }

    private ActionResult ErrorResponse(IQuestionsService.QuestionError error)
    {
        var message = error.ToString();

        return error switch
        {
            IQuestionsService.QuestionError.NotFound => apiErrorFactory.NotFound(message),
            IQuestionsService.QuestionError.InvalidTrackType => apiErrorFactory.BadRequest(message),
            IQuestionsService.QuestionError.InvalidTrack => apiErrorFactory.BadRequest(message),
            IQuestionsService.QuestionError.CannotLoadTrack => apiErrorFactory.BadRequest(message),
            IQuestionsService.QuestionError.InUse => apiErrorFactory.BadRequest(message),
            _ => apiErrorFactory.ServerError(message)
        };
    }
}
