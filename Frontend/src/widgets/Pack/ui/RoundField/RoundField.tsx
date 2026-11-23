import styles from "./RoundField.module.css"
import { AddItem } from "@/shared/ui/AddItem/AddItem.tsx";
import CellTable from "@/widgets/Pack/ui/CellTable/CellTable.tsx";
import { usePackDraftContext } from "@/widgets/Pack/model/PackDraftContext.tsx";

const RoundField = ( { idx } : { idx : number }) => {
  const draft = usePackDraftContext();
  const round = draft.pack.rounds[idx];

  return (
      <div className={styles.field}>
        <table className={styles.gameTable}>
          <thead>
            <tr>
              <th className={styles.cornerCell}>Категория / Цена</th>
              {round.costs.map((cost, cIdx) => (
                <th key={cIdx} className={styles.costTh}>
                  <div className={styles.thContent}>
                    <span>{cost}</span>
                    {/* это херня нужна чтобы я не забыл что нужно сделать удаление*/}
                    {/*<span className={styles.costMark} aria-hidden="true" />*/}
                  </div>
                </th>
              ))}
              <th className={styles.addTh}>
                <AddItem
                  label="Добавить стоимость"
                  inputType="number"
                  onSubmit={(value) => draft.addCost(Number(value))}
                />
              </th>
            </tr>
          </thead>
          <tbody>
          {round.categories.map((category, row) => (
            <tr key={row}>
              <td className={styles.catTd}>
                <div className={styles.tdContent}>
                  {/* это херня нужна чтобы я не забыл что нужно сделать удаление*/}
                  {/*<span className={styles.handle} aria-hidden="true">⠿</span>*/}
                  <span className={styles.catName}>{category}</span>
                </div>
              </td>

              {round.costs.map((cost, col) => {
                return (
                  <td key={col} className={styles.cellTd}>
                    <CellTable
                      cell={draft.pack.rounds[idx].questions[row]?.[col] ?? null}
                      cost={cost}
                      category={category}
                      onCellDrop={(cell) => draft.setCell(row, col, cell)}
                    />
                  </td>
                );
              })}
            </tr>
          ))}
          <tr>
            <td className={styles.addTd}>
              <AddItem label="Добавить категорию" onSubmit={draft.addCategory} />
            </td>
          </tr>
          </tbody>
        </table>
    </div>
  );
};

export default RoundField;