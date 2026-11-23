import { useGameByCode } from "@/entities/game";
import { type FormEvent, useEffect, useState } from "react";

import styles from "./JoinByCodeForm.module.css"
import Input from "@/shared/ui/Input/Input.tsx";
import { useNavigate } from "react-router";

const JoinByCodeForm = () => {
  const [code, setCode] = useState("");
  const navigate = useNavigate();
  const [submittedCode, setSubmittedCode] = useState<string | null>(null);
  const { data : url, isError, isFetching } = useGameByCode(submittedCode ?? "");

  const handleChange = (value: string) => {
    const sanitized = value
      .toUpperCase()
      .replace(/[^A-Z0-9]/g, "")
      .slice(0, 6);

    setCode(sanitized);
    setSubmittedCode(null);
  };

  const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (/^[A-Z0-9]{6}$/.test(code)) {
      setSubmittedCode(code);
    }
  };

  useEffect(() => {
    if (url) {
      navigate(url);
    }
  }, [url, navigate]);

  return (
    <form
      className={styles.joinGameForm}
      onSubmit={handleSubmit}
    >
      <div className={styles.joinGameFormByCode}>
        <label className={styles.joinGameFormByCodeLabel}>
          Код комнаты
        </label>
        <Input
          className={styles.joinGameFormByCodeInput}
          name="roomCode"
          placeholder="RRCF84"
          value={code}
          onChange={(event) => handleChange(event.target.value)}
          type="text"
          required
          minLength={6}
          maxLength={6}
          pattern="[A-Z0-9]{6}"
          autoComplete="off"
          autoCapitalize="characters"
          autoCorrect="off"
          spellCheck={false}
        />
      </div>

      {isError && submittedCode && ( <p className={styles.gameNotFound}>Не удалось найти комнату</p>)}

      <button
        className={styles.joinGameFormButton}
        type="submit"
        disabled={code.length !== 6 || isFetching}
      >
        Подключиться
      </button>
    </form>
  );
};

export default JoinByCodeForm;