using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using WinRT;

namespace Strasciierry.UI.Extensions;
[ComImport]
[Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
unsafe interface IMemoryBufferByteAccess
{
    void GetBuffer(out byte* buffer, out uint copacity);
}

public static class SoftwareBitmapExtensions
{
    public static SoftwareBitmap ConvertToGrayscale(this SoftwareBitmap inputBitmap)
    {
        if (inputBitmap == null)
            throw new ArgumentNullException(nameof(inputBitmap), "Input SoftwareBitmap cannot be null.");

        // Создаем новый объект SoftwareBitmap с форматом Gray8 (оттенки серого)
        var outputBitmap = new SoftwareBitmap(BitmapPixelFormat.Gray8, inputBitmap.PixelWidth, inputBitmap.PixelHeight);

        // Преобразуем входной и выходной объекты SoftwareBitmap в буферы
        using var inputBuffer = inputBitmap.LockBuffer(BitmapBufferAccessMode.Read);
        using var outputBuffer = outputBitmap.LockBuffer(BitmapBufferAccessMode.Write);

        using var inputReference = inputBuffer.CreateReference();
        using var outputReference = outputBuffer.CreateReference();

        unsafe
        {
            // Получаем указатели на пиксели во входном и выходном буферах
            inputReference.As<IMemoryBufferByteAccess>().GetBuffer(out byte* inputDataInBytes, out uint inputCopacity);
            outputReference.As<IMemoryBufferByteAccess>().GetBuffer(out byte* outputDataInBytes, out uint outputCopacity);

            // Проходим по каждому пикселю в изображении и преобразуем его в оттенок серого
            for (var y = 0; y < inputBitmap.PixelHeight; y++)
            {
                for (var x = 0; x < inputBitmap.PixelWidth; x++)
                {
                    // Получаем индексы пикселя во входном и выходном массивах данных
                    var inputIndex = y * inputBitmap.PixelWidth * 4 + x * 4;
                    var outputIndex = y * outputBitmap.PixelWidth + x;

                    // Вычисляем значение оттенка серого для текущего пикселя
                    var grayValue = (byte)((inputDataInBytes[inputIndex] + inputDataInBytes[inputIndex + 1] + inputDataInBytes[inputIndex + 2]) / 3);

                    // Записываем значение оттенка серого в выходной массив данных
                    outputDataInBytes[outputIndex] = grayValue;
                }
            }
        }

        return outputBitmap;
    }

    public static SoftwareBitmap Resize(this SoftwareBitmap sourceBitmap, int newWidth, int newHeight)
    {
        if (sourceBitmap == null || newWidth <= 0 || newHeight <= 0)
            throw new ArgumentException("Invalid input parameters");

        var resizedBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, newWidth, newHeight, BitmapAlphaMode.Premultiplied);

        // Преобразуем входной и выходной объекты SoftwareBitmap в буферы
        using var sourceBitmapBuffer = sourceBitmap.LockBuffer(BitmapBufferAccessMode.Read);
        using var resizedBitmapBuffer = resizedBitmap.LockBuffer(BitmapBufferAccessMode.Write);

        // Получаем информацию о размере изображений
        var sourceStride = sourceBitmapBuffer.GetPlaneDescription(0).Stride;
        var resizedStride = resizedBitmapBuffer.GetPlaneDescription(0).Stride;

        // Получаем указатели на начало каждого изображения
        unsafe
        {
            using var sourceReference = sourceBitmapBuffer.CreateReference();
            using var resizedReference = resizedBitmapBuffer.CreateReference();

            // Получаем указатели на пиксели изображений
            sourceReference.As<IMemoryBufferByteAccess>().GetBuffer(out byte* sourceBytes, out uint sourceCopacity);
            resizedReference.As<IMemoryBufferByteAccess>().GetBuffer(out byte* resizedBytes, out uint resizedCopacity);

            // Копируем данные из исходного изображения в новое с измененным размером
            for (var y = 0; y < newHeight; y++)
            {
                for (var x = 0; x < newWidth; x++)
                {
                    var sourceX = (int)((double)x * sourceBitmap.PixelWidth / newWidth);
                    var sourceY = (int)((double)y * sourceBitmap.PixelHeight / newHeight);

                    var sourceIndex = sourceY * sourceStride + 4 * sourceX;
                    var resizedIndex = y * resizedStride + 4 * x;

                    resizedBytes[resizedIndex] = sourceBytes[sourceIndex];
                    resizedBytes[resizedIndex + 1] = sourceBytes[sourceIndex + 1];
                    resizedBytes[resizedIndex + 2] = sourceBytes[sourceIndex + 2];
                    resizedBytes[resizedIndex + 3] = sourceBytes[sourceIndex + 3];
                }
            }
        }

        return resizedBitmap;
    }
}
