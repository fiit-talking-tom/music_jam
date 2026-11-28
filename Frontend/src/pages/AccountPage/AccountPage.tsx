import styles from "./AccountPage.module.css";
import { useNavigate } from "react-router";
import Header from "@/widgets/Header/ui/Header.tsx";
import { useLogout } from "@/features/auth/api/useLogout.ts";

const AccountPage = () => {
  const navigate = useNavigate();
  const logout = useLogout();

  return (
    <div>
      <Header />
      <div className={styles.options}>
        <h1 className={styles.title}>Мой аккаунт</h1>
        <button
          className={styles.logout}
          onClick={() => logout.mutate()}
        >
          Выйти из аккаунта
        </button>
      </div>

      <div className={styles.info}>
        <div
          className={styles.myPacks}
          onClick={() => navigate('/packs')}
        >
          Паки
        </div>
      </div>
    </div>
  );
};

export default AccountPage;