import styles from "./PackField.module.css"
import RoundField from "@/widgets/Pack/ui/RoundField/RoundField.tsx";
import PackQuestionsSection from "@/widgets/Pack/ui/PackQuestionsSection/PackQuestionsSection.tsx";
import { AddItem } from "@/shared/ui/AddItem/AddItem.tsx";
import { usePackDraftContext } from "@/widgets/Pack/model/PackDraftContext.tsx";
import clsx from "clsx";
import { useState } from "react";
import { packSchema, useUpsertPack } from "@/features/pack";

const PackField = () => {
  const draft  = usePackDraftContext();
  const [error, setError] = useState('');
  // Временно, потом будут норм ошибки
  console.log(error)

  const upsert = useUpsertPack();

  const onSave = () => {
    const parsed = packSchema.safeParse(draft.pack);
    if (!parsed.success) return setError(parsed.error.issues[0].message);

    setError('');
    upsert.mutate({ id: draft.packId, ...parsed.data });
  };

  return (
    <>
      <div className={styles.page}>
        <div className={styles.questionSection}>
          <PackQuestionsSection />
        </div>
        <div className={styles.round}>
          <div className={styles.roundSwitcher}>
              {draft.pack.rounds.map((round, index) => {
                const isActive = index === draft.roundIndex;
                return (<button
                  key={index}
                  type="button"
                  role="tab"
                  aria-selected={isActive}
                  className={clsx(styles.roundTab, isActive && styles.roundTabActive)}
                  onClick={() => draft.setRoundIndex(index)}
                >
                  {round.name}
                </button>)
              })}
            <div className={styles.addRound}>
              <AddItem label={"Добавить раунд"} onSubmit={draft.addRound} />
            </div>
          </div>
          <RoundField idx={draft.roundIndex}/>
        </div>
      </div>
      <div className={styles.wrapper}>
        <button
          type="button"
          className={styles.button}
          onClick={onSave}
          disabled={upsert.isPending}
        >
              <span className={styles.content}>
                <img src="/SaveIcon.svg" alt={"Save pack"}/>
                <span className={styles.label}>Сохранить пак</span>
              </span>
        </button>
      </div>
    </>
  );
};

export default PackField;