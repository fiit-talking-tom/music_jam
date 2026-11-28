import styles from "./PackQuestionsSection.module.css"
import { useQuestions } from "@/entities/question";
import { QuestionCard } from "@/entities/question";
import { EditQuestionButton } from "@/features/question/ui/EditQuestionButton/EditQuestionButton.tsx";
import { useState } from "react";
import clsx from "clsx";

type PackQuestionsSectionProps = {
  scrollSide?: "left" | "right";
  className?: string;
};

const PackQuestionsSection = ({
  scrollSide = "left",
  className,
}: PackQuestionsSectionProps) => {
  const { data } = useQuestions();
  const [query, setQuery] = useState("");
  const items = data?.pages.flat() ?? [];
  const q = query.trim().toLowerCase();
  const filtered = q
    ? items.filter((item) => (item.correctAnswer ?? "").toLowerCase().includes(q))
    : items;
  const [openQuestionId, setOpenQuestionId] = useState<string | null>(null);

  return (
    <section className={clsx(styles.section, className)} aria-label="Созданные вопросы">
      <label className={styles.search}>
        <input
          className={styles.searchInput}
          type="search"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Найти существующий вопрос"
        />
        <svg className={styles.searchIcon} viewBox="0 0 23 23" fill="none" aria-hidden="true">
          <circle cx="10" cy="10" r="7" stroke="currentColor" strokeWidth="2.38" />
          <path d="M15.5 15.5L21 21" stroke="currentColor" strokeWidth="2.38" strokeLinecap="round" />
        </svg>
      </label>

      <div
        className={clsx(
          styles.scroll,
          scrollSide === "left" ? styles.scrollLeft : styles.scrollRight
        )}
      >
        <div className={styles.list}>
          {filtered.map((question) => (
            <QuestionCard
              key={question.questionId}
              question={question}
              isOpen={openQuestionId === question.questionId}
              onToggle={() => setOpenQuestionId(
                openQuestionId === question.questionId
                  ? null
                  : question.questionId
              )}
              actions={<EditQuestionButton question={question} />}
            />
          ))}
        </div>
      </div>
    </section>
  );
};

export default PackQuestionsSection;