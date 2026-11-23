import styles from "./PackField.module.css"
import RoundField from "@/widgets/Pack/ui/RoundField/RoundField.tsx";
import PackQuestionsSection from "@/widgets/Pack/ui/PackQuestionsSection/PackQuestionsSection.tsx";
import { AddItem } from "@/shared/ui/AddItem/AddItem.tsx";
import { usePackDraftContext } from "@/widgets/Pack/model/PackDraftContext.tsx";
import clsx from "clsx";

const PackField = () => {
  const draft  = usePackDraftContext();

  return (
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
  );
};

export default PackField;