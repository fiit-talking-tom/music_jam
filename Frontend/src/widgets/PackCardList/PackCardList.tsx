import { PackCard, usePackCardList } from "@/entities/pack";
import styles from "./PackCardList.module.css";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { Button } from "@/shared/ui/Button/Button.tsx";
import { useStartGame } from "@/features/start-game/model/useStartGame.ts";
import { DeleteButton } from "@/features/pack/ui/DeleteButton/DeleteButton";
import { useDeletePack } from "@/features/pack";

const PAGE_SIZE = 12;

const PackCardList = () => {
  const [page, setPage] = useState(0);
  const navigate = useNavigate();
  const { data } = usePackCardList({ skip: page * PAGE_SIZE, take: PAGE_SIZE });
  const create = useStartGame();
  const deletePack = useDeletePack();

  useEffect(() => {
    if (data && data.length === 0 && page > 0) setPage((p) => p - 1);
  }, [data, page]);

  const handleStartGame = async ({ packId } : { packId : string }) => {
    const { gameId } = await create.mutateAsync({ packId });
    navigate(`/games/${gameId}/waiting-room`);
  }

  const items = data ?? []

  return (
    <div className={styles.wrapper}>
      <ul className={styles.packList}>
        {items.map((pack) => (
          <PackCard
            key={pack.packId}
            pack={pack}
            onClick={() => navigate(`/packs/${pack.packId}`)}
            deleteButton={
              <DeleteButton
                isRound={true}
                ariaLabel="Удалить пак"
                title="Удалить пак?"
                description={`Пак «${pack.name}» будет удалён без возможности восстановления.`}
                onDelete={() => deletePack.mutateAsync(pack.packId)}
              />
            }
            startButton={<Button text={"Запустить"} onClick={() => handleStartGame({ packId : pack.packId})}/>}
          />
        ))}
      </ul>
    </div>
  );
};

export default PackCardList;