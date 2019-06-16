# Kaminashi_toremo
大学生の時にデザイナーの友人の卒業制作のお手伝いとして作成したものです。
友人が描いたキャラクターをUnity上で動作させることを目的としており、
下記参考書を元に実装しようとしていたものを引き継いで制作しました。
付属していたサンプルプログラムを作り変える方針でしたので、素早く実現したいことを反映させることができました。

Unityではじめる2Dゲーム作り徹底ガイド スマートフォンでも遊べる本格ゲーム開発に挑戦   大野 功二
https://www.amazon.co.jp/dp/4797376708/ref=cm_sw_r_tw_dp_U_x_bNhbDb9EQEQ96

### 制作メンバー
３人（プログラム、デザイン、サウンド）

### 期間
約３ヶ月

### 環境
MacOSX Yosemite</br>
Unity 5.3.5f

### 作成ファイル
Assets/Scripts以下の下記スクリプト</br>
・KaminashiMainControl.cs</br>
　プレイヤーの入力からどのアクションを行うかを指定</br>
・NewBaseCharacterController.cs</br>
　KaminashiMainControlから呼び出されるアクション関数を定義</br>
・NewPlayerController.cs</br>
　プレイヤーの現在の状態に応じたアニメーションを実行するスクリプト</br>
・SandbagBodyCollider.cs</br>
　配置した攻撃できるサンドバッグの当たり判定に関するスクリプト</br>
・SandbagController.cs</br>
　サンドバッグの動作に関するスクリプト</br>

Assets/Sprites/Animations/以下のアニメーションファイルの作成</br>

### 操作方法
移動：WASD、↑←↓→</br>
ジャンプ：Spaceキー</br>
攻撃A：左Altキー</br>
攻撃B：左Shiftキー</br>

