# ![logo](src/Strasciierry.UI/Assets/StoreLogo.scale-400.png) Strasciierry

Программа для Windows, преобразующая растровую графику в ASCII-арт

## Установка

>[!NOTE]
>Все требования к системе см. в заметках о релизе [тык](https://github.com/SHXNVRD/Strasciierry/releases/latest)
>
| Архитектура | Портативная версия | Установщик |
|:-----------:|:------------------:|:----------:|
| x64 | [Скачать](https://github.com/SHXNVRD/Strasciierry/releases/download/v1.0.0.0/strasciierry-1.0.0.0-portable-x64.7z) | [Скачать](https://github.com/SHXNVRD/Strasciierry/releases/download/v1.0.0.0/strasciierry-1.0.0.0-x64.7z) |
| x86 | [Скачать](https://github.com/SHXNVRD/Strasciierry/releases/download/v1.0.0.0/strasciierry-1.0.0.0-x86.7z) | [Скачать](https://github.com/SHXNVRD/Strasciierry/releases/download/v1.0.0.0/strasciierry-1.0.0.0-x86.7z) |
| arm64 | [Скачать](https://github.com/SHXNVRD/Strasciierry/releases/download/v1.0.0.0/strasciierry-1.0.0.0-portable-arm647z.7z) | [Скачать](https://github.com/SHXNVRD/Strasciierry/releases/download/v1.0.0.0/strasciierry-1.0.0.0-arm64.7z) |

При выборе портативной версии запустите исполняемый файл `.exe` программы из распакованного архива.

Для выполнения инсталяции при помощи установщика пройдите следующие шаги:

1. Дважды кликните на файл с расширением `.cer`
2. В открывшемся окне нажмите "Установить"
3. В мастере импорта сертификатов выберите "Локальный компьютер" или   "Текущий пользователь" . Это действие может потребовать повышения прав администратора
4. В следующем окне нажмите "Далее", тем самым выбрав "Автоматически выбирать хранилище на основе типа сертификата"
5. Нажмите "Готово"
6. Выполните скрипт `install.ps1` с помощью PowerShell

## Удаление
Для удаления остаточных файлов программы, оставшихся после её деинсталяции, удалите директорию `C:\Users\<You>\AppData\Local\Strasciierry`