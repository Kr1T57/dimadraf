# py -3.12 collector.py
import os
from pathlib import Path
import subprocess
import sys

try:
    from fpdf import FPDF
except ImportError:
    print("[!] Библиотека 'fpdf2' не найдена.")
    print("[*] Начинаю автоматическую установку...")
    try:
        subprocess.check_call([sys.executable, "-m", "pip", "install", "fpdf2"])
        print("[+] Библиотека успешно установлена!\n")
        from fpdf import FPDF
    except Exception as err:
        print(f"[x] Ошибка при автоматической установке: {err}")
        print("Попробуйте установить вручную командой: pip install fpdf2")
        sys.exit(1)

OUTPUT_PDF_NAME = "project_source_code.pdf"

IGNORE_DIRS = {
    "node_modules",
    ".git",
    ".vs",
    ".idea",
    ".vscode",
    "bin",
    "obj",
    ".vite",
    "dist",
    "build",
    "__pycache__",
}

IGNORE_EXTENSIONS = {
    ".png",
    ".jpg",
    ".jpeg",
    ".gif",
    ".svg",
    ".ico",
    ".woff",
    ".woff2",
    ".ttf",
    ".eot",
    ".dll",
    ".exe",
    ".pdb",
    ".cache",
    ".vsidx",
    ".suo",
    ".user",
    ".sql",
    ".db",
    ".sqlite",
    ".map",
    ".pdf",
}

IGNORE_FILES = {
    "package-lock.json",
    "pnpm-lock.yaml",
    "yarn.lock",
    "project.assets.json",
    "combined_text.txt",
}

class CodePDF(FPDF):

    def footer(self):
        self.set_y(-15)
        self.set_font("CodeFont", size=8)
        self.set_text_color(130, 130, 130)
        self.cell(0, 10, f"Страница {self.page_no()}/{{nb}}", align="C")


def setup_pdf():
    pdf = CodePDF(orientation="P", unit="mm", format="A4")
    pdf.set_auto_page_break(auto=True, margin=15)

    windir = os.environ.get("WINDIR", "C:\\Windows")
    font_regular = os.path.join(windir, "Fonts", "consola.ttf")
    font_bold = os.path.join(windir, "Fonts", "consolab.ttf")

    pdf.add_font("CodeFont", "", font_regular)
    pdf.add_font("CodeFont", "B", font_bold)

    pdf.add_page()
    return pdf

def combine_files_to_pdf():
    current_script_path = Path(__file__).resolve()
    root_dir = current_script_path.parent
    output_pdf_path = (root_dir / OUTPUT_PDF_NAME).resolve()

    pdf = setup_pdf()
    files_processed = 0

    print("Начинаю сбор чистого кода в PDF...\n")

    for current_path, dirs, files in os.walk(root_dir):
        dirs[:] = [d for d in dirs if d not in IGNORE_DIRS]

        for file in files:
            file_path = (Path(current_path) / file).resolve()

            if file_path == current_script_path or file_path == output_pdf_path:
                continue

            if file in IGNORE_FILES:
                continue
            if file_path.suffix.lower() in IGNORE_EXTENSIONS:
                continue

            rel_path = file_path.relative_to(root_dir)

            try:
                content = file_path.read_text(encoding="utf-8")

                content = content.replace("\t", "    ")

                part = "OTHER"
                if str(rel_path).startswith("app"):
                    part = "FRONTEND (APP)"
                elif str(rel_path).startswith("GroupWorkAPI"):
                    part = "BACKEND (API)"

                pdf.ln(3)
                pdf.set_font("CodeFont", "B", size=8.5)
                pdf.set_fill_color(235, 240, 248)
                pdf.set_text_color(15, 23, 42)

                header_text = f" [{part}] {rel_path}"
                pdf.cell(0, 6.5, text=header_text, fill=True, new_x="LMARGIN", new_y="NEXT")

                pdf.set_font("CodeFont", "", size=7.2)
                pdf.set_text_color(40, 40, 40)
                pdf.ln(1)

                pdf.multi_cell(0, 3.6, text=content)
                pdf.ln(2)

                files_processed += 1
                print(f"[+] [{part}] Добавлен: {rel_path}")

            except UnicodeDecodeError:
                pass
            except Exception as e:
                print(f"[!] Ошибка при чтении {rel_path}: {e}")

    pdf.output(str(output_pdf_path))

    print("\n" + "=" * 50)
    print(f"Готово! PDF успешно создан: {OUTPUT_PDF_NAME}")
    print(f"Всего файлов упаковано: {files_processed}")


if __name__ == "__main__":
    combine_files_to_pdf()