import styles from "./PackField.module.css"
import RoundField from "@/widgets/Pack/ui/RoundField/RoundField.tsx";
import PackQuestionsSection from "@/widgets/Pack/ui/PackQuestionsSection/PackQuestionsSection.tsx";
import { AddItem } from "@/shared/ui/AddItem/AddItem.tsx";
import { usePackDraftContext } from "@/widgets/Pack/model/PackDraftContext.tsx";
import clsx from "clsx";
import { useState } from "react";
import { packSchema, useUpsertPack } from "@/features/pack";
import { DeleteButton } from "@/features/pack/ui/DeleteButton/DeleteButton.tsx";
import { InlineInput } from "@/shared/ui/InlineInput/InlineInput.tsx";

const PackField = () => {
  const draft  = usePackDraftContext();
  const [error, setError] = useState('');
  const [editingIndex, setEditingIndex] = useState<number | null>(null);
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
                const isEditing = index === editingIndex;
                return (
                  <div>
                    {isEditing ? (
                      <InlineInput
                        label="Название раунда"
                        className={styles.input}
                        initialValue={round.name}
                        commitOnBlur
                        onSubmit={(name) => {
                          draft.renameRound(index, name);
                          setEditingIndex(null);
                        }}
                        onCancel={() => setEditingIndex(null)}
                      />
                    ) : (
                      <div className={clsx(styles.roundTab, isActive && styles.roundTabActive)}>
                        <img src="/Edit.svg" alt="Переименовать раунд" onClick={() => setEditingIndex(index)}/>
                        <button
                          className={styles.roundName}
                          key={index}
                          type="button"
                          role="tab"
                          aria-selected={isActive}
                          onClick={() => draft.setRoundIndex(index)}
                        >
                          {round.name}
                        </button>
                        <DeleteButton
                          isRound={true}
                          ariaLabel="Удалить раунд"
                          title="Удалить раунд?"
                          description={`Раунд «${round.name}» будет удалён вместе со всеми вопросами.`}
                          onDelete={() => draft.deleteRound(index)}
                        />
                      </div>
                    )}

                  </div>
                )
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