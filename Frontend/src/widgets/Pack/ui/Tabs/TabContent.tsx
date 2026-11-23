import PackField from "@/widgets/Pack/ui/PackField/PackField.tsx";
import type { Tab } from "@/widgets/Pack/model/types.ts";
import QuestionCreator from "@/widgets/Question/ui/QuestionCreator/QuestionCreator.tsx";

const TabContent = ({ active } : {active : Tab}) => {
  return (
    <div
      role="tabpanel"
      id={`tabpanel-${active}`}
      aria-labelledby={`tab-${active}`}
    >
      {active === "questions"
        ? <QuestionCreator />
        : <PackField />}
    </div>
  )
};

export default TabContent;