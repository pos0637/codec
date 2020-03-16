#include <vector>
#include <iostream>
#include <stdexcept>
#include <string>
#include <msclr\marshal_cppstd.h>
#include "Rectangle.h"

#using "System.Runtime.InteropServices.dll"
#using "System.Drawing.dll"

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Collections::Generic;
using namespace msclr::interop;
using namespace OpenCvSharp;
using namespace Common;
using namespace Repository::Entities;

extern int StaticImageFaceOp(unsigned char* imageData, int width, int height, int step, std::vector<::Rect>& faceRects, std::vector<::Rect>& addfaceRects);
extern int InitVideoEngine(int scale, int faceNum);
extern int UnInitEngine();
extern int ActiveSDK(std::string id, std::string key);

/// <summary>
/// �������SDK�ӿ���
/// </summary>
public ref class FaceDetector {
public:
    static Repository::Entities::Configuration^ configuration;
    static int imageWidth = 0;
    static int imageHeight = 0;
    static int rectX = 0;
    static int rectY = 0;
    static int rectWidth = 0;
    static int rectHeight = 0;
    static Mat^ yuvImg;
    static Mat^ bgrImg;
    static Mat^ maskImg;
    static Mat^ dstImg;

    /// <summary>
    /// ���������������
    /// </summary>
    static bool StartEngine()
    {
        try {
            configuration = Repository::Repository::LoadConfiguation();
            // ������ֵ����ʾ����С�����ߴ磬�óߴ���������ߴ������ͼƬ���ߵ�ռ�ȣ���Чֵ��Χ[2, 32], �Ƽ�ֵΪ 16
            int scaleOfFace = configuration->ai->faceDetectorParameter->scaleOfFace;
            // ��Ҫ������������
            int numOfFace = configuration->ai->faceDetectorParameter->numberOfFace;
            std::string id = (const char*)(Marshal::StringToHGlobalAnsi(configuration->ai->faceDetectorParameter->appId)).ToPointer();
            std::string key = (const char*)(Marshal::StringToHGlobalAnsi(configuration->ai->faceDetectorParameter->sdkKey)).ToPointer();

            int result = ActiveSDK(id, key);
            if ((result != 0) && (result != 90114)) {
                return false;
            }

            result = InitVideoEngine(scaleOfFace, numOfFace);
            if (result != 0) {
                return false;
            }

            return true;
        }
        catch (std::runtime_error err) {
            Tracker::LogE(gcnew String(err.what()), nullptr, nullptr, 0, nullptr);
            return false;
        }
    }

    /// <summary>
    /// �������ͼ���е������������������ȶԽ����ͨ��������
    /// </summary>
    /// <param name="imageData">ͼ��</param>
    /// <param name="width">����ͼ����</param>
    /// <param name="height">����ͼ��߶�</param>
    /// <param name="length">ͼ�����ݳ���</param>
    /// <param name="roi">����Ȥ����</param>
    static DetectionResult^ DetectFace(IntPtr imageData, int width, int height, int length, Drawing::Rectangle^ roi)
    {
        try {
            if ((width != imageWidth) || (height != imageHeight)) {
                yuvImg = gcnew Mat(height + height / 2, width, MatType::CV_8UC1);
                bgrImg = gcnew Mat(height, width, MatType::CV_8UC3);
                dstImg = gcnew Mat(height, width, MatType::CV_8UC3);
                imageWidth = width;
                imageHeight = height;
            }

            System::Buffer::MemoryCopy(imageData.ToPointer(), yuvImg->Data.ToPointer(), (long long)length, (long long)length);
            Cv2::CvtColor(yuvImg, bgrImg, ColorConversionCodes::YUV2BGR_YV12, 0);

            if ((width % 4) != 0) {
                Cv2::Resize(bgrImg, bgrImg, Size(width - (width % 4), height), 0, 0, InterpolationFlags::Linear);
            }

            if ((roi->X != rectX) || (roi->Y != rectY) || (roi->Width != rectWidth) || (roi->Height != rectHeight)) {
                maskImg = gcnew Mat(height, width, MatType::CV_8UC3, Scalar(0));

                if ((roi->Width != 0) && (roi->Height != 0)) {
                    Cv2::Rectangle(maskImg, OpenCvSharp::Rect(roi->X, roi->Y, roi->Width, roi->Height), Scalar(255, 255, 255), -1, LineTypes::Link8, 0);
                }
                else {
                    Cv2::Rectangle(maskImg, OpenCvSharp::Rect(0, 0, width, height), Scalar(255, 255, 255), -1, LineTypes::Link8, 0);
                }

                rectX = roi->X;
                rectY = roi->Y;
                rectWidth = roi->Width;
                rectHeight = roi->Height;
            }

            bgrImg->CopyTo(dstImg, maskImg);

            std::vector<::Rect> faceRects;
            std::vector<::Rect> AddfaceRects;
            int result = StaticImageFaceOp((unsigned char*)dstImg->Data.ToPointer(), dstImg->Width, dstImg->Height, dstImg->Channels(), faceRects, AddfaceRects);
            if (result != 0) {
                return nullptr;
            }

            DetectionResult^ detectionResult = gcnew DetectionResult();

            for (size_t i = 0; i < faceRects.size(); i++) {
                System::Drawing::Rectangle^ rect = gcnew  System::Drawing::Rectangle((int)faceRects.at(i).left, (int)faceRects.at(i).top, (int)(faceRects.at(i).right - faceRects.at(i).left), (int)(faceRects.at(i).bottom - faceRects.at(i).top));
                detectionResult->rectangles->Add((System::Drawing::Rectangle)rect);
            }

            for (size_t i = 0; i < AddfaceRects.size(); i++) {
                System::Drawing::Rectangle^ addRect = gcnew  System::Drawing::Rectangle((int)AddfaceRects.at(i).left, (int)AddfaceRects.at(i).top, (int)(AddfaceRects.at(i).right - AddfaceRects.at(i).left), (int)(AddfaceRects.at(i).bottom - AddfaceRects.at(i).top));
                detectionResult->addRectangles->Add((System::Drawing::Rectangle)addRect);
            }

            return detectionResult;
        }
        catch (std::runtime_error err) {
            Tracker::LogE(gcnew String(err.what()), nullptr, nullptr, 0, nullptr);
            return nullptr;
        }
    }

    /// <summary>
    /// �ر������������
    /// </summary>
    static bool StopEngine()
    {
        return (UnInitEngine() == 0);
    }
};
