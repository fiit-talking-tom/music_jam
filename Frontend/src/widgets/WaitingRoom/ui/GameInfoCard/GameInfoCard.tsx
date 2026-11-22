import styles from "./GameInfoCard.module.css";
import { useParams } from "react-router-dom";
import { useInviteInfoGame } from "@/entities/game";

export const GameInfoCard = () => {
  const { gameId } = useParams<{ gameId: string }>();
  const { data : inviteInfo, isLoading, isError } = useInviteInfoGame(gameId ?? "");
  console.log("inviteInfo", inviteInfo);
  const copy = (text: string) => {
    navigator.clipboard?.writeText(text);
  };

  if (isLoading) return <section className={styles.card}>Загрузка…</section>;
  if (isError || !inviteInfo) return <section className={styles.card}>Не удалось загрузить данные игры</section>;

  return (
    <section className={styles.card}>
      <h2 className={styles.packName}>
        Название пака
      </h2>

      <div className={styles.schedule}>
        <div className={styles.scheduleItem}>
          <span>Дата</span>
          <div className={styles.value}>
            Дата
          </div>
        </div>

        <div className={styles.scheduleItem}>
          <span>Время</span>
          <div className={styles.value}>
            16.11.2026
          </div>
        </div>
      </div>

      <div className={styles.description}>
        <h3>Описание игры</h3>
        <p>Бебебе</p>
      </div>

      <div className={styles.author}>
        <img
          src="/IconAccount.svg"
          alt="IconAccount"
        />

        <span>Имя автора</span>
      </div>

      <div className={styles.mode}>
        <span>Режим игры:</span>
        <span className={styles.modeValue}>
          С ведущим
        </span>
      </div>

      <div className={styles.invite}>
        <div className={styles.inviteItem}>
          <span>Код приглашения</span>
          <div className={styles.copyField}>
            <div className={`${styles.value} ${styles.copyValue}`}>
              {inviteInfo!.inviteCode}
            </div>
            <button
              type="button"
              className={styles.copyButton}
              onClick={() => copy(inviteInfo!.inviteCode)}
              aria-label="Скопировать код приглашения"
            >
              Копировать
            </button>
          </div>
        </div>

        <div className={styles.inviteItem}>
          <span>Ссылка-приглашение</span>
          <div className={styles.copyField}>
            <div
              className={`${styles.value} ${styles.copyValue} ${styles.link}`}
              title={inviteInfo!.inviteLink}
            >
              {inviteInfo!.inviteLink}
            </div>
            <button
              type="button"
              className={styles.copyButton}
              onClick={() => copy(inviteInfo!.inviteLink)}
              aria-label="Скопировать ссылку-приглашение"
            >
              Копировать
            </button>
          </div>
        </div>
      </div>
    </section>
  );
};