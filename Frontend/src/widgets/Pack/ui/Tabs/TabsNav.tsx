import clsx from "clsx";
import type { Tab } from "@/widgets/Pack/model/types.ts";
import styles from "./TabsNav.module.css";

const TABS: { id: Tab; label: string }[] = [
  { id: "field", label: "Поле" },
  { id: "questions", label: "Вопросы" },
];

const TabsNav = ({ active, onChange }: { active: Tab; onChange: (t: Tab) => void }) => {
  return (
    <div className={styles.nav} role="tablist">
      {TABS.map(({ id, label }) => {
        const isActive = active === id;
        return (
          <button
            key={id}
            type="button"
            role="tab"
            id={`tab-${id}`}
            aria-selected={isActive}
            aria-controls={`tabpanel-${id}`}
            className={clsx(styles.tab, isActive && styles.tabActive)}
            onClick={() => onChange(id)}
          >
            <span className={styles.label}>{label}</span>
          </button>
        );
      })}
    </div>
  );
};

export default TabsNav;