import { useState } from "react";
import { ConfirmDialog } from "@/shared/ui/ConfirmDialog/ConfirmDialog";

type DeleteButtonProps = {
  onDelete: () => void | Promise<unknown>;
  title: string;
  description: string;
  ariaLabel?: string;
  isRound : boolean;
};

export const DeleteButton = ({
  onDelete,
  title,
  description,
  ariaLabel = "Удалить",
  isRound,
}: DeleteButtonProps) => {
  const [open, setOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const handleConfirm = async () => {
    try {
      setIsLoading(true);
      await onDelete();
      setOpen(false);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      <img src={isRound ? "/Trash.svg" : "/Cross.svg" } alt={ariaLabel} onClick={() => setOpen(true)}/>
      <ConfirmDialog
        open={open}
        title={title}
        description={description}
        isLoading={isLoading}
        onConfirm={handleConfirm}
        onCancel={() => setOpen(false)}
      />
    </>
  );
};