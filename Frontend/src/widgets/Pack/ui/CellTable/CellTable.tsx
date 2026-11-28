import style from "./CellTable.module.css"
import { useState } from "react";
import type { DragEvent } from 'react';
import clsx from "clsx";
import type { QuestionDtoCard } from "@/entities/question";
import type { CellDraft } from "@/widgets/Pack/model/draft.ts";

const CELL_MIME = "application/x-pack-cell";

type CellPos = { row: number; col: number };

type CellTableProps = {
  row: number,
  col: number,
  cost : number
  category : string
  cell: CellDraft | null;
  onCellDrop: (cell: CellDraft) => void,
  onCellMove: (from: CellPos, to: CellPos) => void;
  onCellClear: () => void;
}

const CellTable = ({ row, col, cost, category, cell, onCellDrop, onCellMove, onCellClear } : CellTableProps) => {
  const [isDragOver, setIsDragOver] = useState(false);
  const [isDragging, setIsDragging] = useState(false);

  const isFilled = !!cell?.questionId;
  const answer = cell?.correctAnswer ?? null;

  const handleDragStart = (e: DragEvent<HTMLDivElement>) => {
    if (!isFilled) {
      e.preventDefault();
      return;
    }
    e.dataTransfer.setData(CELL_MIME, JSON.stringify({ row, col }));
    e.dataTransfer.effectAllowed = 'move';
    setIsDragging(true);
  };

  const handleDragEnd = (e: DragEvent<HTMLDivElement>) => {
    setIsDragging(false);

    if (e.dataTransfer.dropEffect !== 'none') return;

    const field = e.currentTarget.closest('table')?.parentElement;
    if (field) {
      const r = field.getBoundingClientRect();
      const inside =
        e.clientX >= r.left && e.clientX <= r.right &&
        e.clientY >= r.top && e.clientY <= r.bottom;
      if (inside) return;
    }

    onCellClear();
  };

  const handleDragOver = (e: DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    const isCellMove = e.dataTransfer.types.includes(CELL_MIME);
    e.dataTransfer.dropEffect = isCellMove ? 'move' : 'copy';
    setIsDragOver(true);
  };

  const handleDragLeave = (e: DragEvent<HTMLDivElement>) => {
    if (!e.currentTarget.contains(e.relatedTarget as Node | null)) {
      setIsDragOver(false);
    }
  };

  const handleDrop = (e: DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    setIsDragOver(false);

    const cellData = e.dataTransfer.getData(CELL_MIME);
    if (cellData) {
      const from = JSON.parse(cellData) as CellPos;
      if (from.row === row && from.col === col) return;
      onCellMove(from, { row, col });
      return;
    }

    const data = e.dataTransfer.getData('application/json');
    if (!data) return;

    const dropped = JSON.parse(data) as QuestionDtoCard;

    onCellDrop({
      cost,
      category,
      questionId: dropped.questionId,
      correctAnswer: dropped.correctAnswer ?? null,
    });
  };

  return (
    <div
      className={clsx(
        style.cell,
        isFilled && style.filled,
        isDragOver && style.dragOver,
        isDragging && style.dragging,
      )}
      title={answer ?? "Ячейка"}
      draggable={isFilled}
      onDragStart={handleDragStart}
      onDragEnd={handleDragEnd}
      onDragOver={handleDragOver}
      onDragLeave={handleDragLeave}
      onDrop={handleDrop}
    >
      {isFilled && (
        <div className={style.content}>
          <img src="/Note.svg" alt="Note" />
          {answer && (
            <div className={style.texts}>
              <span className={style.title}>{answer}</span>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default CellTable;