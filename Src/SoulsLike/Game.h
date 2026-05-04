#pragma once

#include "../Xultaik/GameBase.h"

namespace SoulsLike
{
    class Game : public Xultaik::GameBase
    {
    public:
        Game();

    protected:
        void OnInitialize() override;
        void OnUpdate(double deltaSeconds) override;
        void OnRender() override;

    private:
        double m_elapsedTime = 0.0;
    };
}
