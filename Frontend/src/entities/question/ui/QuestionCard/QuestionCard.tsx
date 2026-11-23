import { type ReactNode, useState } from "react";
import styles from "./QuestionCard.module.css"
import type { DragEvent } from 'react';
import type { QuestionDtoCard } from "@/entities/question";
import { SongPlayer } from "@/features/song/ui/SongPlayer.tsx";
import { getAnswerSongUrl, getQuestionSongUrl } from "@/entities/song/model/getSongUrl.ts";
import clsx from "clsx";

type QuestionItemProps = {
  question: QuestionDtoCard;
  actions?: ReactNode;
}

export const QuestionCard = ({ question, actions } : QuestionItemProps) => {
  const [open, setOpen] = useState(false);
  const handleDragStart = (e: DragEvent<HTMLDivElement>) => {
    e.dataTransfer.setData(
      "application/json", JSON.stringify(question));
    e.dataTransfer.effectAllowed = 'copy';
  };

  return (
    <div
      className={clsx(styles.questionItem, open && styles.open)}
      draggable
      onDragStart={handleDragStart}
    >
      <div className={styles.head}>
        <div className={styles.texts}>
          <span className={styles.title}>{question.correctAnswer}</span>
        </div>
        <button
          type="button"
          className={clsx(styles.toggle, open && styles.toggleOpen)}
          aria-expanded={open}
          aria-label={open ? "Свернуть" : "Развернуть"}
          onClick={() => setOpen((v) => !v)}
        >
          <svg viewBox="0 0 20 12" fill="none" aria-hidden="true">
            <path
              d="M2 2L10 10L18 2"
              stroke="currentColor"
              strokeWidth="2.5"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
          </svg>
        </button>
      </div>
      {open && (
        <div className={styles.body}>
          <SongPlayer title="Вопрос" songUrl={getQuestionSongUrl(question.questionId)} />
          <SongPlayer title="Ответ" songUrl={getAnswerSongUrl(question.questionId)} />
          {actions}
        </div>
      )}
    </div>
  );
};