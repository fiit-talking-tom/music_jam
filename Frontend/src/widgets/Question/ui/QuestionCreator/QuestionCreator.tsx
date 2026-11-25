import styles from "./QuestionCreator.module.css"
import { QuestionForm } from "@/features/question";
import PackQuestionsSection from "@/widgets/Pack/ui/PackQuestionsSection/PackQuestionsSection.tsx";
import { useTracks } from "@/features/song/api/api.ts";
import TrackPanel from "@/widgets/Question/ui/TrackPanel/TrackPanel.tsx";


const QuestionCreator = () => {
  const tracks = useTracks();

  return (
    <div className={styles.createForm}>
      <section className={styles.column}>
        <div className={styles.name}>Создание вопроса</div>
        <QuestionForm tracks={tracks.items} onRemove={tracks.remove}/>
      </section>
      <section className={`${styles.column} ${styles.dropColumn}`}>
        <TrackPanel tracks={tracks.items} onAdd={tracks.add} onRemove={tracks.remove} />
      </section>
      <section className={styles.column}>
        <PackQuestionsSection />
      </section>
    </div>
  );
};

export default QuestionCreator;