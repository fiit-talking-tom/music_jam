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

const PackEditor = ({ packId } : { packId : string }) => {
  const { data, error } = usePack(packId);
  if (axios.isAxiosError(error) && error.response?.status != 404) return <div>Не удалось загрузить пак</div>;
  return <PackEditorForm key={data?.packId ?? 'new_pack'} packId={packId} initial={data} />;
};

export const PackEditorForm = ({ packId, initial }: { packId: string; initial?: PackDto }) => {
  const draft = usePackDraft(packId, initial);
  const [activeTab, setActiveTab] = useState<Tab>('field');


  return (
    <PackDraftProvider draft={draft}>
      <div className={styles.page}>
        <div className={styles.options}>
          <div className={styles.packName}>
            {/*Я не придумал ниче лучше*/}
            <input
              value={draft.pack.name}
              className={styles.title}
              onChange={(e) => {
                draft.setName(e.target.value);
              }}
              placeholder={"Название пака"}
            />
          </div>
          <TabsNav active={activeTab} onChange={setActiveTab} />
        </div>
        <TabContent active={activeTab} />
      </div>
    </PackDraftProvider>
  );
};

export default PackEditor;