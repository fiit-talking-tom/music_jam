import { useForm } from "react-hook-form";
import { getQuestionSchema, type QuestionFormValues } from '../../model/schema.ts';
import { zodResolver } from "@hookform/resolvers/zod";
import Input from "@/shared/ui/Input/Input.tsx";
import styles from "./QuestionForm.module.css";
import type { QuestionDtoCard } from "@/entities/question";
import { useCreateQuestion } from "@/features/question";
import { useUpdateQuestion } from "@/features/question";
import { useDeleteQuestion } from "@/features/question";
import type { Track } from "@/features/song/model/types.ts";
import FileField from "@/widgets/Question/ui/FileField/FileField.tsx";

type QuestionFormProps = {
  question?: QuestionDtoCard;
  onSuccess?: () => void;
  tracks?: Track[];
}

export const QuestionForm = ( { question, onSuccess, tracks = [] } : QuestionFormProps) => {
  const isEdit = !!question;
  const { register, handleSubmit, reset, formState : { errors } } = useForm<QuestionFormValues>({
    resolver: zodResolver(getQuestionSchema(isEdit)),
    defaultValues: {
      requested: question?.requested ?? '',
      correctAnswer: question?.correctAnswer ?? '',
    },
  })
  const create = useCreateQuestion();
  const update = useUpdateQuestion();
  const deleteFn = useDeleteQuestion();

  const isPending = create.isPending || update.isPending;

  const onSubmit = (data : QuestionFormValues) => {
    if (isEdit) {
      update.mutate({ id : question.questionId, ...data}, {
        onSuccess: () => onSuccess?.()
      })
    } else {
      create.mutate(data, {
        onSuccess: () => {
          reset();
          onSuccess?.();
        },
      })
    }
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <div className={styles.form}>
        <div className={styles.fields}>
          <FileField
            id="q-track"
            label="Трек вопроса"
            registration={register("questionTrack")}
            error={errors.questionTrack?.message as string | undefined}
            tracks={tracks}
          />
          <FileField
            id="a-track"
            label="Трек ответа"
            registration={register("answerTrack")}
            error={errors.answerTrack?.message as string | undefined}
            tracks={tracks}
          />

          <div>
            <div className={styles.field}>
              <Input
                id="q-requested"
                placeholder={"Что нужно назвать"}
                {...register("requested")}
              />
            </div>
            {errors.requested && <span className={styles.error}>{errors.requested.message}</span>}
          </div>

          <div>
            <div className={styles.field}>
              <Input
                id="q-answer"
                placeholder={"Правильный ответ"}
                {...register("correctAnswer")}
              />
            </div>
            {errors.correctAnswer && <span className={styles.error}>{errors.correctAnswer.message}</span>}
          </div>
        </div>

        <div className={styles.options}>
          {!isEdit && (
            <button
              type="button"
              className={styles.btn}
              onClick={() => reset()}
            >
              Сбросить
            </button>
          )}
          <button
            id="q-submit"
            disabled={isPending}
            type="submit"
            className={`${styles.btn} ${styles.btnPrimary}`}
          >
            {isEdit ? "Сохранить вопрос" : "Создать вопрос"}
          </button>
          {isEdit && (
            <button
              type="button"
              className={styles.btn}
              aria-label="Удалить вопрос"
              onClick={() =>
                deleteFn.mutate(question.questionId, { onSuccess: () => onSuccess?.() })
              }
            >
              <img src="/Trash.svg" alt="Delete question"/>
            </button>
          )}
        </div>
      </div>
    </form>
  );
};