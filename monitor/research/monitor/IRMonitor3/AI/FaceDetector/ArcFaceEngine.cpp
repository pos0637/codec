#include <string>
#include <vector>
#include <iostream>
#include "free/arcsoft_face_sdk.h"
#include "Rectangle.h"

using namespace std;

// 人脸区域放大比率
#define SCALE (1.1F)

static MHandle m_hEngine; // 人脸检测引擎
static int peopleNum = -1; // 上一次已通过人数

// 验证SDK
int ActiveSDK(string id, string key)
{
    char appID[MAX_PATH];
    char sdkKey[MAX_PATH];
    strcpy_s(appID, id.c_str());
    strcpy_s(sdkKey, key.c_str());

    return ASFOnlineActivation(appID, sdkKey);
}

// 关闭识别引擎
int UnInitEngine()
{
    // 销毁引擎
    ASFUninitEngine(m_hEngine);
    m_hEngine = NULL;

    return 0;
}

// 启动视频识别引擎
int InitVideoEngine(int scale, int faceNum)
{
    if (m_hEngine) {
        UnInitEngine();
    }

    return ASFInitEngine(ASF_DETECT_MODE_VIDEO, ASF_OP_0_ONLY, (MInt32)scale, (MInt32)faceNum, ASF_FACE_DETECT | ASF_FACERECOGNITION, &m_hEngine);
}

// 人脸检测算法
int StaticImageFaceOp(unsigned char* imageData, int width, int height, int step, vector<Rect>& faceRects, vector<Rect>& addfaceRects)
{
    try {
        if (imageData == nullptr) {
            return -1;
        }

        ASVLOFFSCREEN offscreen = { 0 };
        offscreen.u32PixelArrayFormat = (unsigned int)ASVL_PAF_RGB24_B8G8R8;
        offscreen.i32Width = width;
        offscreen.i32Height = height;
        offscreen.pi32Pitch[0] = width * step;
        offscreen.ppu8Plane[0] = imageData;

        ASF_MultiFaceInfo faceInfo = { 0 };
        int detectRes = ASFDetectFacesEx(m_hEngine, &offscreen, &faceInfo);

        if (detectRes != 0 || faceInfo.faceNum < 1) {
            return -1;
        }

        int max = 0;
        int maxArea = 0;

        for (int i = 0; i < faceInfo.faceNum; i++) {
            if (faceInfo.faceRect[i].left < 0)
                faceInfo.faceRect[i].left = 10;
            if (faceInfo.faceRect[i].top < 0)
                faceInfo.faceRect[i].top = 10;
            if (faceInfo.faceRect[i].right < 0 || faceInfo.faceRect[i].right >width)
                faceInfo.faceRect[i].right = width - 10;
            if (faceInfo.faceRect[i].bottom < 0 || faceInfo.faceRect[i].bottom > height)
                faceInfo.faceRect[i].bottom = height - 10;

            if ((faceInfo.faceRect[i].right - faceInfo.faceRect[i].left) *
                (faceInfo.faceRect[i].bottom - faceInfo.faceRect[i].top) > maxArea) {
                max = i;
                maxArea = (faceInfo.faceRect[i].right - faceInfo.faceRect[i].left) *
                    (faceInfo.faceRect[i].bottom - faceInfo.faceRect[i].top);
            }
        }

        for (int i = 0; i < faceInfo.faceNum; i++) {
            // 新增人脸区域
            if (faceInfo.faceID[i] > peopleNum) {
                int right = (int)(faceInfo.faceRect[i].right + (faceInfo.faceRect[i].right - faceInfo.faceRect[i].left));
                int left = (int)(faceInfo.faceRect[i].left - (faceInfo.faceRect[i].right - faceInfo.faceRect[i].left));
                int bottom = (int)(faceInfo.faceRect[i].bottom + (faceInfo.faceRect[i].bottom - faceInfo.faceRect[i].top));
                int top = (int)(faceInfo.faceRect[i].top - (faceInfo.faceRect[i].bottom - faceInfo.faceRect[i].top));

                if (bottom > height) {
                    bottom = height - 1;
                }
                if (top < 0) {
                    top = 0;
                }
                if (right > width) {
                    right = width - 1;
                }
                if (left < 0) {
                    left = 0;
                }

                Rect rect = { left, top, right, bottom };
                addfaceRects.push_back(rect);
            }

            // 现有人脸区域
            int bottom = (int)(faceInfo.faceRect[i].bottom * SCALE);
            int top = (int)(faceInfo.faceRect[i].top / SCALE);
            if (bottom > height) {
                bottom = height - 1;
            }

            Rect rect = { faceInfo.faceRect[i].left, top, faceInfo.faceRect[i].right, bottom };
            faceRects.push_back(rect);
        }

        peopleNum = faceInfo.faceID[faceInfo.faceNum - 1];
        return 0;
    }
    catch (const std::exception&) {
        return -1;
    }
}
