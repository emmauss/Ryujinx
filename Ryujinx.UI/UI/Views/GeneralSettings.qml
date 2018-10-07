import QtQuick 2.0
import QtQuick.Controls 2.3
import QtQuick.Layouts 1.3

Frame {
    id: generalSetttingsFrame

    Column {
        anchors.fill: parent

        GroupBox {
            title: qsTr("Game")

            Column {
                CheckBox {
                    id: dockedCheckBox
                    text: "Enable Docked Mode"

                }

                CheckBox {
                    id: vsyncCheckBox
                    text: "Enable Ingame VSync"

                }
            }
        }

        GroupBox {
            title: qsTr("System")

            Column {
                CheckBox {
                    id: memoryCheckBox
                    text: "Enable Memory Checks"

                }

                CheckBox {
                    id: multiCoreCheckBox
                    text: "Enable Multi-Core Scheduling"

                }
            }
        }
    }

}
