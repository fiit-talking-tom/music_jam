import { useRef, useState } from "react";
import styles from "./TrackPanel.module.css"
import { type Track, TRACK_DND_TYPE } from "@/features/song/model/types.ts";
import * as React from "react";

type Props = {
  tracks : Track[];
  onAdd: (files : File[]) => void;
  onRemove: (id : string) => void;
}

const TrackPanel = ({ tracks, onAdd, onRemove } : Props) => {
  const inputRef = useRef<HTMLInputElement>(null);
  const audioRef = useRef<HTMLAudioElement>(null);
  const [dragOver, setDragOver] = useState(false);
  const [playingId, setPlayingId] = useState<string | null>(null);

  const openPicker = () => inputRef.current?.click();

  const togglePlay = (track: Track) => {
    const audio = audioRef.current;
    if (!audio) return;
    if (playingId === track.id) {
      audio.pause();
      setPlayingId(null);
      return;
    }
    audio.src = track.url;
    audio.play().catch(() => setPlayingId(null));
    setPlayingId(track.id);
  };

  const handleRemove = (id: string) => {
    if (playingId === id) {
      audioRef.current?.pause();
      setPlayingId(null);
    }
    onRemove(id);
  };

  const handleDragStart = (e: React.DragEvent, track: Track) => {
    e.dataTransfer?.setData(TRACK_DND_TYPE, track.id);
    e.dataTransfer?.setData("text/plain", track.file.name);
    e.dataTransfer!.effectAllowed = "copy";
  };

  const isFileDrag = (e: React.DragEvent) => {
    return e.dataTransfer?.types.includes("Files") && !e.dataTransfer.types.includes(TRACK_DND_TYPE);
  }

  return (
    <div
      className={`${styles.panel} ${dragOver ? styles.over : ""}`}
      onDragOver={(e : React.DragEvent<HTMLDivElement>) => {
        if (!isFileDrag(e)) return;
        e.preventDefault();
        setDragOver(true);
      }}
      onDragLeave={(e) => {
        if (!e.currentTarget.contains(e.relatedTarget as Node)) setDragOver(false);
      }}
      onDrop={(e) => {
        if (!isFileDrag(e)) return;
        e.preventDefault();
        setDragOver(false);
        onAdd(Array.from(e.dataTransfer.files));
      }}
    >
      <audio
        ref={audioRef}
        onEnded={() => setPlayingId(null)}
        hidden
      />

      <input
        ref={inputRef}
        type="file"
        accept="audio/*"
        multiple
        hidden
        onChange={(e) => {
          onAdd(Array.from(e.target.files ?? []));
          e.target.value = "";
        }}
      />

      {tracks.length > 0 && (
        <ul className={styles.list}>
          {tracks.map((track: Track, i : number) => {
            const isPlaying = playingId === track.id;
            return (
              <li
                key={track.id}
                className={`${styles.item} ${isPlaying ? styles.playing : ""}`}
                draggable
                onDragStart={(e) => handleDragStart(e, track)}
              >
                <button
                  type="button"
                  className={styles.iconBtn}
                  onClick={() => togglePlay(track)}
                  aria-label={isPlaying ? "Пауза" : "Воспроизвести"}
                >
                  {isPlaying ? <PauseIcon /> : <PlayIcon />}
                </button>

                <span className={styles.name} title={track.file.name}>
                  {i + 1} {track.file.name}
                </span>

                <button
                  type="button"
                  className={styles.iconBtn}
                  onClick={() => handleRemove(track.id)}
                  aria-label="Удалить"
                >
                  <TrashIcon />
                </button>
              </li>
            );
          })}
        </ul>
      )}
      <button type="button" className={styles.add} onClick={openPicker}>
        <img src="/Upload.svg" alt="Upload tracks" />
        <span className={styles.addText}>Загрузите или перетащите файлы</span>
      </button>
    </div>
  );
};

const PlayIcon = () => (
  <svg viewBox="0 0 19 19" width="100%" height="100%" aria-hidden>
    <path d="M5 3.5v12l10-6z" fill="#fff" />
  </svg>
);
const PauseIcon = () => (
  <svg viewBox="0 0 19 19" width="100%" height="100%" aria-hidden>
    <rect x="4" y="3.5" width="3.8" height="12" fill="#32C76A" />
    <rect x="11.2" y="3.5" width="3.8" height="12" fill="#32C76A" />
  </svg>
);
const TrashIcon = () => (
  <svg viewBox="0 0 19 19" width="100%" height="100%" fill="none" stroke="#F8F8F8"
       strokeWidth="1.4" strokeLinecap="round" aria-hidden>
    <path d="M3.5 5h12M7.5 5V3.5h4V5M5 5l.7 10.5h7.6L14 5" />
  </svg>
);

export default TrackPanel;