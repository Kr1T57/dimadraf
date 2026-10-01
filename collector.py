import os
from pathlib import Path
import subprocess
import sys

# =====================================================================
# 1. АВТОМАТИЧЕСКАЯ ПРОВЕРКА И УСТАНОВКА БИБЛИОТЕКИ fpdf2
# =====================================================================
try:
    from fpdf import FPDF
except ImportError:
    print("[!] Библиотека 'fpdf2' не найдена.")
    print("[*] Начинаю автоматическую установку...")
    try:
        # Устанавливаем именно в тот Python, который сейчас запущен
        subprocess.check_call([sys.executable, "-m", "pip", "install", "fpdf2"])
        print("[+] Библиотека успешно установлена!\n")
        from fpdf import FPDF
    except Exception as err:
        print(f"[x] Ошибка при автоматической установке: {err}")
        print("Попробуйте установить вручную командой: pip install fpdf2")
        sys.exit(1)


# =====================================================================
# 2. НАСТРОЙКИ И ИСКЛЮЧЕНИЯ
# =====================================================================
OUTPUT_PDF_NAME = "project_source_code.pdf"

# Папки, которые полностью пропускаем
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

# Расширения файлов, которые не нужны в коде
IGNORE_EXTENSIONS = {
    # Графика, шрифты, медиа
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
    # Бинарники, базы данных и кэш
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
    # Карты и PDF-файлы (включая итоговый)
    ".map",
    ".pdf",
}

# Точечные системные файлы для исключения
IGNORE_FILES = {
    "package-lock.json",
    "pnpm-lock.yaml",
    "yarn.lock",
    "project.assets.json",
    "combined_text.txt",
}


# =====================================================================
# 3. КЛАСС PDF С НУМЕРАЦИЕЙ
# =====================================================================
class CodePDF(FPDF):

    def footer(self):
        self.set_y(-15)
        self.set_font("CodeFont", size=8)
        self.set_text_color(130, 130, 130)
        # {nb} автоматически превращается в общее число страниц
        self.cell(0, 10, f"Страница {self.page_no()}/{{nb}}", align="C")


def setup_pdf():
    pdf = CodePDF(orientation="P", unit="mm", format="A4")
    pdf.set_auto_page_break(auto=True, margin=15)

    # Ищем стандартный шрифт Consolas (есть на всех Windows)
    windir = os.environ.get("WINDIR", "C:\\Windows")
    font_regular = os.path.join(windir, "Fonts", "consola.ttf")
    font_bold = os.path.join(windir, "Fonts", "consolab.ttf")

    # Регистрируем шрифт с поддержкой кириллицы
    pdf.add_font("CodeFont", "", font_regular)
    pdf.add_font("CodeFont", "B", font_bold)

    pdf.add_page()
    return pdf


# =====================================================================
# 4. ОСНОВНАЯ ФУНКЦИЯ СБОРА
# =====================================================================
def combine_files_to_pdf():
    # Точные пути для исключения самого себя и итогового файла
    current_script_path = Path(__file__).resolve()
    root_dir = current_script_path.parent
    output_pdf_path = (root_dir / OUTPUT_PDF_NAME).resolve()

    pdf = setup_pdf()
    files_processed = 0

    print("Начинаю сбор чистого кода в PDF...\n")

    for current_path, dirs, files in os.walk(root_dir):
        # 1. Отсекаем мусорные папки (os.walk даже не пойдет внутрь)
        dirs[:] = [d for d in dirs if d not in IGNORE_DIRS]

        for file in files:
            file_path = (Path(current_path) / file).resolve()

            # 2. ЖЕСТКАЯ ЗАЩИТА: Не читаем сам скрипт и итоговый PDF
            if file_path == current_script_path or file_path == output_pdf_path:
                continue

            # 3. Фильтр по именам и расширениям
            if file in IGNORE_FILES:
                continue
            if file_path.suffix.lower() in IGNORE_EXTENSIONS:
                continue

            rel_path = file_path.relative_to(root_dir)

            try:
                content = file_path.read_text(encoding="utf-8")

                # Табуляции заменяем на пробелы (PDF не умеет правильно выравнивать \t)
                content = content.replace("\t", "    ")

                # Маркировка директории (Фронтенд / Бэкенд)
                part = "OTHER"
                if str(rel_path).startswith("app"):
                    part = "FRONTEND (APP)"
                elif str(rel_path).startswith("GroupWorkAPI"):
                    part = "BACKEND (API)"

                # Шапка блока с файлом (голубовато-серый фон)
                pdf.ln(3)
                pdf.set_font("CodeFont", "B", size=8.5)
                pdf.set_fill_color(235, 240, 248)
                pdf.set_text_color(15, 23, 42)

                header_text = f" [{part}] {rel_path}"
                pdf.cell(0, 6.5, text=header_text, fill=True, new_x="LMARGIN", new_y="NEXT")

                # Печать текста кода
                pdf.set_font("CodeFont", "", size=7.2)
                pdf.set_text_color(40, 40, 40)
                pdf.ln(1)

                pdf.multi_cell(0, 3.6, text=content)
                pdf.ln(2)

                files_processed += 1
                print(f"[+] [{part}] Добавлен: {rel_path}")

            except UnicodeDecodeError:
                # Бинарные файлы молча пропускаем
                pass
            except Exception as e:
                print(f"[!] Ошибка при чтении {rel_path}: {e}")

    # Сохранение готового документа
    pdf.output(str(output_pdf_path))

    print("\n" + "=" * 50)
    print(f"Готово! PDF успешно создан: {OUTPUT_PDF_NAME}")
    print(f"Всего файлов упаковано: {files_processed}")


if __name__ == "__main__":
    combine_files_to_pdf()