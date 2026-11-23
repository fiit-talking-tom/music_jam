import type { PackDto } from "@/entities/pack";
import { usePackDraft } from "@/widgets/Pack/model/usePackDraft.ts";
import { useState } from "react";
import type { Tab } from "@/widgets/Pack/model/types.ts";
import { PackDraftProvider } from "@/widgets/Pack/model/PackDraftContext.tsx";
import TabsNav from "@/widgets/Pack/ui/Tabs/TabsNav.tsx";
import TabContent from "@/widgets/Pack/ui/Tabs/TabContent.tsx";
import styles from "./PackEditor.module.css"
import { usePack } from "@/entities/pack/api/pack.queries.ts";
import axios from "axios";
import { packSchema, useUpsertPack } from "@/features/pack";

const PackEditor = ({ packId } : { packId : string }) => {
  const { data, error } = usePack(packId);
  if (axios.isAxiosError(error) && error.response?.status != 404) return <div>Не удалось загрузить пак</div>;
  return <PackEditorForm key={data?.packId ?? 'new_pack'} packId={packId} initial={data} />;
};

export const PackEditorForm = ({ packId, initial }: { packId: string; initial?: PackDto }) => {
  const draft = usePackDraft(packId, initial);
  const [activeTab, setActiveTab] = useState<Tab>('field');

  const [error, setError] = useState('');

  const upsert = useUpsertPack();
  console.log(error);

  const onSave = () => {
    const parsed = packSchema.safeParse(draft.pack);
    if (!parsed.success) return setError(parsed.error.issues[0].message);

    setError('');
    upsert.mutate({ id: draft.packId, ...parsed.data });
  };

  const isNew = !initial;
  return (
    <PackDraftProvider draft={draft}>
      <div className={styles.page}>
        <div className={styles.options}>
          <input
            value={draft.pack.name}
            className={styles.title}
            onChange={(e) => {
              draft.setName(e.target.value);
            }}
            placeholder={"Название пака"}
          />
          <TabsNav active={activeTab} onChange={setActiveTab} />
        </div>
        <TabContent active={activeTab} />
        <div className={styles.wrapper}>
          <button
            type="button"
            className={styles.button}
            onClick={onSave}
            disabled={upsert.isPending}
          >
            <span className={styles.content}>
              <img src="/SaveIcon.svg" alt={"Save pack"}/>
              <span className={styles.label}>{isNew ? 'Создать' : 'Сохранить'} пак</span>
            </span>
          </button>
        </div>
      </div>
    </PackDraftProvider>
  );
};

export default PackEditor;