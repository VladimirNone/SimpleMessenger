   M S B A M L   `   `   `  ���� G  CWpfMessenger, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WpfMessengerWpfMessenger      WpfMessenger.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:WpfMessenger       � Title$  	Messenger��$	��594��$	��804�� � 
ResizeMode$ NoResize=� � SizeToContent$ WidthAndHeight=�.��� -    �Name$ MainGrid�� � ColumnDefinitions �� $��* �$	��100���� $��3* �$	��100��.+�� -   $ gChats��$��0��$y�Visible=�.+��� -    ��Name$ bSignOut��$��Sign out��$��50��$��20��$��10,9,141,549q��� -   $ bBack��$
��Back��$��50��$��20��$��141,9,10,549q�$y�Hidden=��� -    ��Name$ tbFieldForFindUser��$��10,37,35,519q��� -   $��166,37,10,519q�$
��🔍���� $
��Auto=�$��Hidden=�$��10,58,10,33q�.���� -    ��Name$ lbListOfChats��$	��482���� -   $ bAddParticipantsToChat��$��20��$	��181��$��10,545,10,13q�$��Add participants to chat��� -	   $ gContentOfChats��$��2��$��
204,0,0,-9q�$y�Visible=�.+��� -
   $��Show more messages��$��
10,10,10,0q�$��20��$	��Top=��� -   $ lbChatHistory��$��Hidden=�$
��Auto=�   Messages#���   $��10,30,10,56q����� .R�$� $��Bottom=����� .R�� $	��550�� �� $��* �$��50���� $
��0.5* �$��50���� $��* �$��50��.+��� $
��Wrap=� �� Padding$ 	30 0 30 0q�
����������� *	ShortInfo+	 �� FallbackValue$	 Error��

 � Tag
 �� *SendingUser.NickName+���� *Column+ �� TextAlignment �� *TextAlignment+�� -   $ tbFieldForMessage��$��30��$	��550��$��10,534,40,20q��� -   $��30��$��30��$��560,534,10,20q�$��18(�$	��→��� -   $ gEnter��$y�Hidden=�$��2��.+��� $��Center=�$��Center=�
�������$	��200��$	��170��.+��� -   $ tbLoginNickName��$	��100��$��10 20 10 10q��� -   $ tbLoginPassword��$	��100��$��0q��� -   $ bLoginEnter��$��Log in��$��50��$��10q��� $��Or you can sign up��$ Center=��� -   $ bSigninEnter��$��Sign up��$��50��$��10q�