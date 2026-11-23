import { type Track, TRACK_DND_TYPE } from "@/features/song/model/types.ts";
import { useEffect, useRef, useState } from "react";
import { type UseFormRegisterReturn } from "react-hook-form";
import styles from "./FileField.module.css"
import Input from "@/shared/ui/Input/Input.tsx";

type FileFieldProps = {
  id: string;
  label: string;
  error?: string;
  registration: UseFormRegisterReturn;
  tracks: Track[];
};

const FileField = ({ id, label, error, registration, tracks }: FileFieldProps) => {
  const [over, setOver] = useState(false);
  const [fileName, setFileName] = useState<string>();
  const fieldRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const form = fieldRef.current?.closest("form");
    if (!form) return;
    const onReset = () => setFileName(undefined);
    form.addEventListener("reset", onReset);
    return () => form.removeEventListener("reset", onReset);
  }, []);

  const accepts = (e: React.DragEvent) =>
    e.dataTransfer.types.includes(TRACK_DND_TYPE) || e.dataTransfer.types.includes("Files");

  const handleDrop = (e: React.DragEvent) => {
    if (!accepts(e)) return;
    e.preventDefault();
    setOver(false);
    const trackId = e.dataTransfer.getData(TRACK_DND_TYPE);
    const file = trackId
      ? tracks.find((t) => t.id === trackId)?.file
      : e.dataTransfer.files[0];
    if (!file || !file.type.startsWith("audio/")) return;
    const input = e.currentTarget.querySelector<HTMLInputElement>('input[type="file"]');
    if (!input) return;

    const dt = new DataTransfer();
    dt.items.add(file);
    input.files = dt.files;
    input.dispatchEvent(new Event("change", { bubbles: true }));
  };

  return (
    <div>
      <div
        ref={fieldRef}
        className={`${styles.field} ${over ? styles.fieldOver : ""}`}
        onDragOver={(e) => {
          console.log("ЕБАЛ", e.dataTransfer.types)
          if (!accepts(e)) return;
          e.preventDefault();
          e.dataTransfer.dropEffect = "copy";
          setOver(true);
        }}
        onDragLeave={(e) => {
          if (!e.currentTarget.contains(e.relatedTarget as Node)) setOver(false);
        }}
        onDrop={handleDrop}
      >
        <label htmlFor={id} className={styles.fieldText}>
          {fileName ?? label}
        </label>
        <Input
          id={id}
          type="file"
          accept="audio/*"
          {...registration}
          onChange={(e) => {
            registration.onChange(e);
            setFileName(e.target.files?.[0]?.name);
          }}
        />
      </div>
      {error && <span className={styles.error}>{error}</span>}
    </div>
  );
};
export default FileField;