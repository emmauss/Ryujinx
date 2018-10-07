import QtQuick 2.4
import QtQuick.Window 2.11
import QtQuick.Layouts 1.3
import QtQuick.Templates 2.4
import QtQuick.Controls 2.3

Window {
    id: configWindow
    width: 640
    height: 480
    color: "#f0efef"
    title: "Configuration"
    modality: Qt.ApplicationModal

    Column{
        spacing: 10
        id: contentColumn
        height: configWindow.height
        width: configWindow.width

        TabBar {
            id: configTabs
            currentIndex: 0
            height: 50
            width: parent.width

            TabButton {
                width: 80
                height: 50
                text: qsTr("General")
                checked: true
            }

            /*TabButton {
                text: qsTr("Controls")
            }*/

            TabButton {
                x: 80
                y: 0
                width: 80
                text: qsTr("Misc")
                checked: false
                height: 50
            }
        }

        StackLayout {
            id: pagesView
            currentIndex: configTabs.currentIndex
            width: contentColumn.width
            height: contentColumn.height
                    - configTabs.height
                    - bottomRow.height
                    - (contentColumn.spacing * 3)

            Item {
                id: genConfigPage

                Loader {
                    source: "./GeneralSettings.qml"

                    width: pagesView.width
                    height: pagesView.height
                }
            }

            Item {
                id: miscConfigPage

                Loader {
                    source: "./MiscSettings.qml"
                    width: pagesView.width
                    height: pagesView.height
                }
            }

            /*onIndexChanged: {
                if(currentIndex !== configContent.currentIndex){
                    configContent.currentIndex = currentIndex
                }
            }*/
        }

        Row {
            spacing: 20
            id: bottomRow
            anchors.right: parent.right
            anchors.rightMargin: 55

            Button{
                id: acceptButton
                width: 80
                height: 20
                text: "OK"

                onClicked: {

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

                    configWindow.close()
                }
            }
       }
    }
}
