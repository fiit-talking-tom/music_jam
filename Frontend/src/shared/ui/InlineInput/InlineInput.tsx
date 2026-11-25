import { useRef, type KeyboardEvent } from "react";
import styles from "./InlineInput.module.css"
import clsx from "clsx";

type InlineInputProps = {
  label: string;
  inputType?: "text" | "number";
  initialValue?: string;
  className?: string;
  commitOnBlur?: boolean;
  onSubmit: (value: string) => void;
  onCancel: () => void;
};

export const InlineInput = ({
  label,
  className,
  inputType = "text",
  initialValue = "",
  commitOnBlur = false,
  onSubmit,
  onCancel,
}: InlineInputProps) => {
  const done = useRef(false);

  const finish = (raw: string, save: boolean) => {
    if (done.current) return;
    done.current = true;
    const value = raw.trim();
    if (save && value && value !== initialValue) onSubmit(value);
    else onCancel();
  };

  const handleKeyDown = (e: KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") finish(e.currentTarget.value, true);
    if (e.key === "Escape") finish(e.currentTarget.value, false);
  };

  return (
    <input
      autoFocus
      className={clsx(styles.input, className)}
      type={inputType}
      defaultValue={initialValue}
      placeholder={label}
      aria-label={label}
      onFocus={(e) => e.currentTarget.select()}
      onKeyDown={handleKeyDown}
      onBlur={(e) => finish(e.currentTarget.value, commitOnBlur)}
    />
  );
};