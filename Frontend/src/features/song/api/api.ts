import { useCallback, useEffect, useRef, useState } from "react";
import type { Track } from "@/features/song/model/types.ts";

export function useTracks() {
  const [items, setItems] = useState<Track[]>([]);
  const itemsRef = useRef<Track[]>([]);
  itemsRef.current = items;

  const add = useCallback((files: File[]) => {
    const audio = files.filter((f) => f.type.startsWith("audio/"));
    if (!audio.length) return;

    setItems((prev) => [
      ...prev,
      ...audio.map((file) => ({
        id: crypto.randomUUID(),
        file,
        url: URL.createObjectURL(file),
      })),
    ]);
  }, []);

  const remove = useCallback((id: string) => {
    const track = itemsRef.current.find((t) => t.id === id);
    if (track) URL.revokeObjectURL(track.url);

    setItems((prev) => prev.filter((t) => t.id !== id));
  }, []);

  useEffect(() =>
      () => itemsRef.current.forEach((t) => URL.revokeObjectURL(t.url)),
    []
  );

  return { items, add, remove };
}