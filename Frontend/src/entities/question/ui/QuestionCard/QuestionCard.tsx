import { type ReactNode } from "react";
import styles from "./QuestionCard.module.css"
import type { DragEvent } from 'react';
import type { QuestionDtoCard } from "@/entities/question";
import { SongPlayer } from "@/features/song/ui/SongPlayer.tsx";
import { getAnswerSongUrl, getQuestionSongUrl } from "@/entities/song/model/getSongUrl.ts";
import clsx from "clsx";

type QuestionItemProps = {
  question: QuestionDtoCard;
  isOpen: boolean;
  onToggle: () => void;
  actions?: ReactNode;
}

export const QuestionCard = ({ question, isOpen, onToggle, actions } : QuestionItemProps) => {
  const handleDragStart = (e: DragEvent<HTMLDivElement>) => {
    e.dataTransfer.setData(
      "application/json", JSON.stringify(question));
    e.dataTransfer.effectAllowed = 'copy';
  };

  return (
    <div
      className={clsx(styles.questionItem, isOpen && styles.open)}
      draggable
      onDragStart={handleDragStart}
    >
      <div className={styles.head}>
        <div className={styles.texts}>
          <span className={styles.title}>{question.correctAnswer}</span>
        </div>
        <button
          type="button"
          className={clsx(styles.toggle, isOpen && styles.toggleOpen)}
          aria-expanded={isOpen}
          aria-label={isOpen ? "Свернуть" : "Развернуть"}
          onClick={onToggle}
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
      {isOpen && (
        <div className={styles.body}>
          <div className={styles.players}>
            <SongPlayer title="Вопрос" songUrl={getQuestionSongUrl(question.questionId)} />
            <SongPlayer title="Ответ" songUrl={getAnswerSongUrl(question.questionId)} />
          </div>

          <div className={styles.field}>
            <span className={styles.fieldLabel}>Что нужно назвать</span>
            <div className={styles.fieldBox}>
              <span className={styles.fieldValue}>{question.requested}</span>
            </div>
          </div>

          <div className={styles.field}>
            <span className={styles.fieldLabel}>Правильный ответ</span>
            <div className={clsx(styles.fieldBox, styles.fieldBoxCorrect)}>
              <span className={styles.fieldValue}>{question.correctAnswer}</span>
            </div>
          </div>

          {actions && <div className={styles.actions}>{actions}</div>}
        </div>
      )}
    </div>
  );
};