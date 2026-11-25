import { useState } from "react";
import styles from "./EditableLabel.module.css";
import { InlineInput } from "@/shared/ui/InlineInput/InlineInput";
import { DeleteButton } from "@/features/pack/ui/DeleteButton/DeleteButton";

type EditableLabelProps = {
  value: string;
  inputType?: "text" | "number";
  renameLabel: string;
  deleteLabel: string;
  deleteTitle: string;
  deleteDescription: string;
  onRename: (value: string) => void;
  onDelete: () => void | Promise<unknown>;
};

export const EditableLabel = ({
  value,
  inputType = "text",
  renameLabel,
  deleteLabel,
  deleteTitle,
  deleteDescription,
  onRename,
  onDelete,
}: EditableLabelProps) => {
  const [isEditing, setIsEditing] = useState(false);

  if (isEditing) {
    return (
      <div className={styles.edit}>
        <InlineInput
          label={renameLabel}
          inputType={inputType}
          initialValue={value}
          commitOnBlur
          onSubmit={(v) => { onRename(v); setIsEditing(false); }}
          onCancel={() => setIsEditing(false)}
        />
      </div>
    );
  }

  return (
    <div className={styles.root}>
      <span
        className={styles.name}
        onClick={() => setIsEditing(true)}
      >
        {value}
      </span>
      <div className={styles.deleteWrap}>
        <DeleteButton
          isRound={false}
          ariaLabel={deleteLabel}
          title={deleteTitle}
          description={deleteDescription}
          onDelete={onDelete}
        />
      </div>
    </div>
  );
};