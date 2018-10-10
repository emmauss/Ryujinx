import QtQuick 2.4
import QtQuick.Window 2.11
import QtQuick.Layouts 1.3
import QtQuick.Templates 2.4
import QtQuick.Controls 2.3
import Ryujinx 1.0

Window {
    id: configWindow
    width: 640
    height: 480
    color: "#f0efef"
    title: "Configuration"
    modality: Qt.ApplicationModal

    onClosing: {
        configModel.dispose()
    }

    ColumnLayout {
        spacing: 10
        id: contentColumn
        anchors.rightMargin: 10
        anchors.leftMargin: 10
        anchors.bottomMargin: 10
        anchors.topMargin: 10
        anchors.fill: parent

        TabBar {
            id: configTabs
            Layout.fillHeight: false
            Layout.fillWidth: true
            currentIndex: 0

            TabButton {
                width: 80
                text: qsTr("General")
            }

            TabButton {
                width: 80
                text: qsTr("Controls")
            }

            TabButton {
                width: 80
                text: qsTr("Misc")
            }
        }

        StackLayout {
            id: pagesView
            Layout.fillHeight: true
            Layout.fillWidth: true
            currentIndex: configTabs.currentIndex


            Item {
                id: genConfigPage
                Layout.fillHeight: true
                Layout.fillWidth: true

                Loader {
                    source: "./GeneralSettings.qml"

                    width: pagesView.width
                    height: pagesView.height
                }
            }

            Item {
                id: controlConfigPage
                Layout.fillHeight: true
                Layout.fillWidth: true

                Loader {
                    source: "./ControlsSettings.qml"

                    width: pagesView.width
                    height: pagesView.height
                }
            }

            Item {
                id: miscConfigPage
                Layout.fillHeight: true
                Layout.fillWidth: true

                Loader {
                    source: "./MiscSettings.qml"
                    width: pagesView.width
                    height: pagesView.height
                }
            }
        }

        RowLayout {
            spacing: 20
            id: bottomRow
            Layout.alignment: Qt.AlignRight | Qt.AlignVCenter
            Layout.fillWidth: true

            Button{
                id: acceptButton
                width: 80
                height: 20
                text: "OK"

                onClicked: {
                    configModel.save()

                    configWindow.close()
                }
            }

            Button{
                id: cancelButton
                width: 80
                height: 20
                text: "Cancel"
                visible: true

                onClicked: {
                    configModel.discard()

                    configWindow.close()
                }
            }
       }
    }

    ConfigurationModel {
        id: configModel
    }
}
