import Header from "@/widgets/Header/ui/Header.tsx";
import styles from "./Packs.module.css"
import PackCardList from "@/widgets/PackCardList/PackCardList.tsx";
import { useNavigate } from "react-router";

const Packs = () => {
  const navigate = useNavigate();

  return (
    <div>
      <Header />
      <div className={styles.packsContent}>
        <div className={styles.packOptions}>
          <div className={styles.titleGroup}>
            <button
              type="button"
              className={styles.back}
              onClick={() => navigate(-1)}
              aria-label="Назад"
            >
              <img src="/BackButton.svg" alt="BackPacks"/>
            </button>
            <h1 className={styles.title}>Паки</h1>
          </div>
          <button
            type="button"
            className={styles.createButton}
            onClick={() => navigate(`/packs/${crypto.randomUUID()}`)}
          >
            <svg width="29" height="29" viewBox="0 0 29 29" fill="none" aria-hidden="true">
              <path
                d="M14.5 6.5V22.5M6.5 14.5H22.5"
                stroke="currentColor"
                strokeWidth="2.56"
                strokeLinecap="round"
              />
            </svg>
            Создать новый пак
          </button>
        </div>
        <PackCardList />
      </div>
    </div>
  );
};

export default Packs;