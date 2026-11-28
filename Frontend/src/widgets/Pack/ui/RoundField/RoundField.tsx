import styles from "./RoundField.module.css"
import { AddItem } from "@/shared/ui/AddItem/AddItem.tsx";
import CellTable from "@/widgets/Pack/ui/CellTable/CellTable.tsx";
import { usePackDraftContext } from "@/widgets/Pack/model/PackDraftContext.tsx";
import { EditableLabel } from "@/shared/ui/EditableLabel/EditableLabel.tsx";

const RoundField = ( { idx } : { idx : number }) => {
  const draft = usePackDraftContext();
  const round = draft.pack.rounds[idx];

  if (!round) return null;

  return (
      <div className={styles.field}>
        <table className={styles.gameTable}>
          <thead>
            <tr>
              <th className={styles.cornerCell}>Категория / Цена</th>
              {round.costs.map((cost, cIdx) => (
                <th key={cIdx} className={styles.costTh}>
                  <div className={styles.thContent}>
                    <EditableLabel
                      value={String(cost)}
                      inputType="number"
                      renameLabel="Изменить стоимость"
                      deleteLabel="Удалить стоимость"
                      deleteTitle="Удалить стоимость?"
                      deleteDescription={`Стоимость ${cost} будет удалена вместе со всеми вопросами в столбце.`}
                      onRename={(v) => {
                        const n = Number(v);
                        if (Number.isFinite(n) && n > 0) draft.renameCost(cIdx, n);
                      }}
                      onDelete={() => draft.deleteCost(cIdx)}
                    />
                  </div>
                </th>
              ))}
              <th className={styles.addTh}>
                <AddItem
                  label="Добавить стоимость"
                  inputType="number"
                  onSubmit={(value) => {
                    const n = Number(value);
                    if (Number.isFinite(n) && n > 0) draft.addCost(n);
                  }}
                  className={styles.inputCost}
                />
              </th>
            </tr>
          </thead>
          <tbody>
          {round.categories.map((category, row) => (
            <tr key={row}>
              <td className={styles.catTd}>
                <div className={styles.tdContent}>
                  <EditableLabel
                    value={category}
                    renameLabel="Переименовать категорию"
                    deleteLabel="Удалить категорию"
                    deleteTitle="Удалить категорию?"
                    deleteDescription={`Категория «${category}» будет удалена вместе со всеми вопросами.`}
                    onRename={(name) => draft.renameCategory(row, name)}
                    onDelete={() => draft.deleteCategory(row)}
                  />
                </div>
              </td>

              {round.costs.map((cost, col) => {
                return (
                  <td key={col} className={styles.cellTd}>
                    <CellTable
                      row={row}
                      col={col}
                      cell={draft.pack.rounds[idx].questions[row]?.[col] ?? null}
                      cost={cost}
                      category={category}
                      onCellDrop={(cell) => draft.setCell(row, col, cell)}
                      onCellMove={(from, to) => draft.moveCell(from, to)}
                      onCellClear={() => draft.clearCell(row, col)}
                    />
                  </td>
                );
              })}
            </tr>
          ))}
          <tr>
            <td className={styles.addTd}>
              <AddItem
                label="Добавить категорию"
                onSubmit={draft.addCategory}
                className={styles.inputCategory}
              />
            </td>
          </tr>
          </tbody>
        </table>
    </div>
  );
};

export default RoundField;