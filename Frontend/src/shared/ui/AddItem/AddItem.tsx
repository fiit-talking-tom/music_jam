import { useState } from 'react';
import styles from './AddItem.module.css';
import { InlineInput } from "@/shared/ui/InlineInput/InlineInput.tsx";

interface AddItemProps {
  label: string;
  inputType?: 'text' | 'number';
  onSubmit: (value: string) => void;
}

export const AddItem = ({ label, inputType = 'text', onSubmit }: AddItemProps) => {
  const [isEditing, setIsEditing] = useState(false);

  if (!isEditing) {
    return (

      <button
        type="button"
        className={styles.addButton}
        title={label}
        aria-label={label}
        onClick={() => setIsEditing(true)}
      >
        <img
          src="/Plus.svg"
          alt="Add Item Icon"
          className={styles.addIcon}
        />
      </button>
    );
  }

  return (
    <InlineInput
      label={label}
      className={styles.input}
      inputType={inputType}
      onSubmit={(v) => { onSubmit(v); setIsEditing(false); }}
      onCancel={() => setIsEditing(false)}
    />
  );
};