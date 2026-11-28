import type { PackCardDto } from "@/entities/pack";
import styles from "./PackCard.module.css";
import { type ReactNode} from "react";

type PackCardProps = {
  pack: PackCardDto;
  onClick?: () => void;
  deleteButton?: ReactNode;
  startButton?: ReactNode;
}

const rules = new Intl.PluralRules("ru-RU");

const plural = (n: number, one: string, few: string, many: string) => {
  const form = rules.select(n);
  return form === "one" ? one : form === "few" ? few : many;
};

const formatDate = (iso: string) => new Date(iso).toLocaleDateString("ru-RU");
const formatTime = (iso: string) =>
  new Date(iso).toLocaleTimeString("ru-RU", { hour: "2-digit", minute: "2-digit" });

export const PackCard = ({ pack, onClick, deleteButton, startButton }: PackCardProps) => {
  return (
    <div className={styles.packCard} onClick={onClick}>
      <div className={styles.header}>
        <span className={styles.iconSlot}>
          <img src="/Edit.svg" alt="Edit"/>
        </span>
        <h2 className={styles.name}>{pack.name}</h2>
        <div className={styles.iconSlot} onClick={(e) => e.stopPropagation()}>
          {deleteButton}
        </div>
      </div>
      <div className={styles.stats}>
        <span>
          <b className={styles.accent}>{pack.roundCount}</b>{" "}
          {plural(pack.roundCount, "тур", "тура", "туров")}
        </span>
        <span>
          <b className={styles.accent}>{pack.totalQuestionCount}</b>{" "}
          {plural(pack.totalQuestionCount, "вопрос", "вопроса", "вопросов")}
        </span>
      </div>
      <div className={styles.dates}>
        <div className={styles.dateBlock}>
          <div className={styles.dateTitle}>Дата создания</div>
          <time className={styles.dateValue} dateTime={pack.createdAtUtc}>
            {formatDate(pack.createdAtUtc)}
          </time>
        </div>
        <div className={styles.dateBlock}>
          <div className={styles.dateTitle}>Последнее изменение</div>
          <time className={styles.dateValue} dateTime={pack.updatedAtUtc}>
            <span className={styles.time}>{formatTime(pack.updatedAtUtc)}</span>{" "}
            {formatDate(pack.updatedAtUtc)}
          </time>
        </div>
      </div>
      {startButton && (
        <div className={styles.start} onClick={(e) => e.stopPropagation()}>
          {startButton}
        </div>
      )}
    </div>
  );
};
