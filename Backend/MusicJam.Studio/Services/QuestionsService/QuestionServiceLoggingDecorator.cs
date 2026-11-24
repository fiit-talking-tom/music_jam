using System.Diagnostics;
using MusicJam.Studio.Models.Packs;
using MusicJam.Studio.Models.Tracks;
using MusicJam.Studio.Requests.Questions;
using MusicJam.Studio.Responses;
using MusicJam.Studio.Responses.Questions;
using MusicJam.Studio.Telemetry;
using Shared;

namespace MusicJam.Studio.Services.QuestionsService;

public class QuestionServiceLoggingDecorator(
    IQuestionsService questionsService,
    ILogger<QuestionServiceLoggingDecorator> logger) : IQuestionsService
{
    public async Task<Result<QuestionCardResponse[], IQuestionsService.QuestionError>> GetQuestions(Guid userId, GetQuestionsRequest request)
    {
        using var activity = StudioActivitySource.Source.StartActivity("QuestionService.GetQuestions");
        activity?.SetTag("user.id", userId);
        
        logger.LogInformation(
            "GetQuestions questiuons with userId = {userId}",
            userId);
        var questionResult = await questionsService.GetQuestions(userId, request);
        if (questionResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            
            logger.LogInformation(
                "GetQuestions with user = {userId} finished with fault = {StatusCode}",
                userId,
                error);

            return error;
        }
        
        logger.LogInformation(
            "GetQuestions with user = {userId} finished successfully. AllShortPacks length = {Length}",
            userId,
            token.Length);
        
        return questionResult;
    }

    public async Task<Result<Question, IQuestionsService.QuestionError>> Create(Guid userId, CreateQuestionRequest request)
    {
        using var activity = StudioActivitySource.Source.StartActivity("QuestionService.Create");
        activity?.SetTag("user.id", userId);
        
        logger.LogInformation(
            "Create question with userId = {userId}",
            userId);
        var questionResult = await questionsService.Create(userId, request);
        if (questionResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            
            logger.LogInformation(
                "Create question with user = {userId} finished with fault = {StatusCode}",
                userId,
                error);

            return error;
        }
        
        logger.LogInformation(
            "Create question with user = {userId} finished successfully. QuestionId = {Id}",
            userId,
            token.Id);
        
        return questionResult;
    }

    public async Task<Result<Question, IQuestionsService.QuestionError>> Get(Guid userId, Guid questionId)
    {
        using var activity = StudioActivitySource.Source.StartActivity("QuestionService.Get");
        activity?.SetTag("user.id", userId);
        activity?.SetTag("question.id", questionId);
        
        logger.LogInformation(
            "Get question with userId = {userId} and questionId = {questionId}",
            userId,
            questionId);
        
        var questionResult = await questionsService.Get(userId, questionId);
        if (questionResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            
            logger.LogInformation(
                "Get question with userId = {userId} and questionId = {questionId} finished with fault = {StatusCode}",
                userId,
                questionId,
                error);

            return error;
        }
        
        logger.LogInformation(
            "Get question with userId = {userId} and questionId = {questionId} finished successfully. QuestionId = {Id}",
            userId,
            questionId,
            token.Id);
        
        return questionResult;
    }

    public async Task<Result<Guid, IQuestionsService.QuestionError>> Delete(Guid userId, Guid questionId)
    {
        using var activity = StudioActivitySource.Source.StartActivity("QuestionService.Delete");
        activity?.SetTag("user.id", userId);
        activity?.SetTag("question.id", questionId);
        
        logger.LogInformation(
            "Delete question with userId = {userId} and questionId = {questionId}",
            userId,
            questionId);
        
        var questionResult = await questionsService.Delete(userId, questionId);
        if (questionResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            
            logger.LogInformation(
                "Delete question with userId = {userId} and questionId = {questionId} finished with fault = {StatusCode}",
                userId,
                questionId,
                error);

            return error;
        }
        
        logger.LogInformation(
            "Delete question with userId = {userId} and questionId = {questionId} finished successfully. QuestionId = {Id}",
            userId,
            questionId,
            token);
        
        return questionResult;
    }

    public async Task<Result<Question, IQuestionsService.QuestionError>> Update(Guid userId, Guid questionId, PatchMode mode, UpdateQuestionRequest request)
    {
        using var activity = StudioActivitySource.Source.StartActivity("QuestionService.Update");
        activity?.SetTag("user.id", userId);
        activity?.SetTag("question.id", questionId);
        
        logger.LogInformation(
            "Update question with userId = {userId} and questionId = {questionId}, patchMode = {mode}",
            userId,
            questionId,
            mode);
        
        var questionResult = await questionsService.Update(userId, questionId, mode, request);
        if (questionResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            
            logger.LogInformation(
                "Update question with userId = {userId} and questionId = {questionId}, patchMode = {mode} finished with fault = {StatusCode}",
                userId,
                questionId,
                mode,
                error);

            return error;
        }
        
        logger.LogInformation(
            "Update question with userId = {userId} and questionId = {questionId}, patchMode = {mode} finished successfully. QuestionId = {Id}",
            userId,
            questionId,
            mode,
            token.Id);
        
        return questionResult;
    }

    public async Task<Result<TrackResponse, IQuestionsService.QuestionError>> GetTrack(Guid userId, Guid questionId, TrackType trackType, long? start, long? end)
    {
        using var activity = StudioActivitySource.Source.StartActivity("QuestionService.GetTrack");
        activity?.SetTag("user.id", userId);
        activity?.SetTag("question.id", questionId);
        
        logger.LogInformation(
            "GetTrack question with userId = {userId} and questionId = {questionId}",
            userId,
            questionId);
        
        var questionResult = await questionsService.GetTrack(userId, questionId, trackType, start, end);
        if (questionResult.TryGetFault(out var error, out var token))
        {
            activity?.SetStatus(ActivityStatusCode.Error, error.ToString());
            
            logger.LogInformation(
                "GetTrack question with userId = {userId} and questionId = {questionId} finished with fault = {StatusCode}",
                userId,
                questionId,
                error);

            return error;
        }
        
        logger.LogInformation(
            "GetTrack question with userId = {userId} and questionId = {questionId} finished successfully. TrackId = {Id}",
            userId,
            questionId,
            token.TrackId);
        
        return questionResult;
    }
}