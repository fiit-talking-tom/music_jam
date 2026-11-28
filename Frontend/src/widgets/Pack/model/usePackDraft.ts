import { useState } from 'react';
import type { PackDto } from "@/entities/pack";
import type { CellDraft, PackDraftState, RoundDraft } from "@/widgets/Pack/model/draft.ts";
import { emptyCell, emptyRound, toDraft } from "@/widgets/Pack/model/mappers.ts";

type CellPos = { row: number; col: number };

export const usePackDraft = (packId: string, initial? : PackDto) => {
  const [pack, setPack] = useState<PackDraftState>(() => toDraft(initial));
  const [roundIndex, setRoundIndex] = useState(0);

  const updateRound = (index: number, update: (r: RoundDraft) => RoundDraft) =>
    setPack((p) => ({
      ...p,
      rounds: p.rounds.map((r, i) => (i === index ? update(r) : r)),
    }));

  const clearCell = (row: number, col: number) =>
    updateRound(roundIndex, (r) => ({
      ...r,
      questions: r.questions.map((line, y) =>
        y === row
          ? line.map((c, x) => (x === col ? emptyCell(r.costs[col], r.categories[row]) : c))
          : line
      ),
    }));

  const moveCell = (from: CellPos, to: CellPos) =>
    updateRound(roundIndex, (r) => {
      if (from.row === to.row && from.col === to.col) return r;

      const a = r.questions[from.row]?.[from.col];
      const b = r.questions[to.row]?.[to.col];
      if (!a || !b) return r;

      const next = r.questions.map((line) => [...line]);
      next[to.row][to.col] = { ...a, cost: r.costs[to.col], category: r.categories[to.row] };
      next[from.row][from.col] = { ...b, cost: r.costs[from.col], category: r.categories[from.row] };

      return { ...r, questions: next };
    });

  const setCell = (row: number, col: number, cell: CellDraft) =>
    setPack((p) => ({
      ...p,
      rounds: p.rounds.map((r, i) =>
        i !== roundIndex
          ? r
          : {
            ...r,
            questions: r.questions.map((line, y) =>
              y === row ? line.map((c, x) => (x === col ? cell : c)) : line
            ),
          }
      ),
    }));

  const addRound = (name: string) => {
    setRoundIndex(pack.rounds.length);
    setPack((p) => ({ ...p, rounds: [...p.rounds, emptyRound(name)] }));
  };

  const addCost = (cost: number) =>
    updateRound(roundIndex, (r) => ({
      ...r,
      costs: [...r.costs, cost],
      questions: r.questions.map((row, i) => [...row, emptyCell(cost, r.categories[i])]),
    }));

  const addCategory = (name: string) =>
    updateRound(roundIndex, (r) => ({
      ...r,
      categories: [...r.categories, name],
      questions : [...r.questions, r.costs.map((cost) => emptyCell(cost, name))]
    }));

  const renameCategory = (index: number, newName: string) =>
    updateRound(roundIndex, (r) => {
      if (r.categories.some((c, i) => i !== index && c === newName)) return r; // дубликат
      return {
        ...r,
        categories: r.categories.map((c, i) => (i === index ? newName : c)),
        questions: r.questions.map((line, y) =>
          y === index ? line.map((c) => ({ ...c, category: newName })) : line
        ),
      };
    });

  const renameCost = (index: number, newCost: number) =>
    updateRound(roundIndex, (r) => {
      if (r.costs.some((c, i) => i !== index && c === newCost)) return r;
      return {
        ...r,
        costs: r.costs.map((c, i) => (i === index ? newCost : c)),
        questions: r.questions.map((line) =>
          line.map((c, x) => (x === index ? { ...c, cost: newCost } : c))
        ),
      };
    });

  const renameRound = (index: number, name: string) =>
    setPack((p) => ({
      ...p,
      rounds: p.rounds.map((r, i) => (i === index ? { ...r, name } : r)),
    }));

  const deleteCategory = (index: number) =>
    updateRound(roundIndex, (r) => ({
      ...r,
      categories: r.categories.filter((_, i) => i !== index),
      questions: r.questions.filter((_, y) => y !== index),
    }));

  const deleteCost = (index: number) =>
    updateRound(roundIndex, (r) => ({
      ...r,
      costs: r.costs.filter((_, i) => i !== index),
      questions: r.questions.map((line) => line.filter((_, x) => x !== index)),
    }));

  const deleteRound = (index: number) => {
    setPack((p) => ({ ...p, rounds: p.rounds.filter((_, i) => i !== index) }));
    setRoundIndex((cur) => Math.max(0, cur > index || cur === pack.rounds.length - 1 ? cur - 1 : cur));
  };

  const setName = (name: string) => setPack((p) => ({ ...p, name }));


  return {
    packId,
    pack,
    round : pack.rounds[roundIndex],
    roundIndex,
    setRoundIndex,
    setCell,
    clearCell,
    moveCell,
    setName,
    addRound,
    addCost,
    addCategory,
    renameRound,
    renameCost,
    renameCategory,
    deleteRound,
    deleteCost,
    deleteCategory
  };
};
export type PackDraft = ReturnType<typeof usePackDraft>;