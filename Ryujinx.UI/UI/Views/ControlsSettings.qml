import QtQuick 2.0
import QtQuick.Controls 2.3
import QtQuick.Layouts 1.3

Frame {
    id: miscSettingsFrame
    width: 500
    height: 480

    ColumnLayout{
        id: column
        spacing: 5
        anchors.fill: parent

        TabBar {
            id: controllerTypesBar
            currentIndex: 0

            TabButton {
                width: 80
                text: "Keyboard"
            }

            TabButton {
                width: 80
                text: "GamePad"
            }
        }

        StackLayout {
            id: controllerStack
            currentIndex: controllerTypesBar.currentIndex
            clip: true

            Item {
                id: keyboardSettings
                Layout.fillHeight: true
                Layout.fillWidth: true

                ScrollView {
                    anchors.fill: parent

                    Loader {
                        anchors.fill: parent
                        source: "./KeyboardSettings.qml"

                        width: controllerStack.width
                        height: controllerStack.height
                    }
                }
            }

            Item {
                id: gamePadSettings
                Layout.fillHeight: true
                Layout.fillWidth: true

                ScrollView {
                    anchors.fill: parent

                    Loader {
                        anchors.fill: parent
                        source: "./GamePadSettings.qml"

                        width: controllerStack.width
                        height: controllerStack.height
                    }
                }
            }
        }
    }
}
