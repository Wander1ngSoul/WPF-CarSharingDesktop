   M S B A M L   `   `   `  ����?  ;Lab5, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:Lab5.WindowsLab5.Windows  5   	        Lab5.Windows.ClientsWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:Lab5.Windows     5   	     � Title$'  !Страница клиентов��5   	   $	��450��6#   $	��800��60   ��5	      �� 5
   
    �� 
StartPoint$	 0,0&�6    �� EndPoint$	 1,1&�6/   .��5      �  � Color$ #2C3E50��6    � Offset$ 0Q�6+   � 5      $ #34495E��6   $ 1Q�6+   5      .��5      � $��20q�6   .+�5   
   �� $#��Список клиентов��6   $��22(�6+   $
��Bold"�69   
�������$��Center=�5   '   $
��0,10q�6D   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ ClientsDataGrid��5         AutoGenerateColumns .  $��Stretch=�5      $��Stretch=�61   $
��0,20q�5      $	��280��6!      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Имя��6%   	   Width5  /System.Windows.Controls.DataGridLengthConverter$	 * 6P   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 62   �� *	FirstName+6P     5      $ Фамилия��6%   $	 * 6S   
 66   �� *
LastName+6S     5      $ Отчество��6%   $	 * 6V   
 67   �� *
MiddleName+6V     5      $ Email��6%   $	 * 6N   
 64   �� *Email+6N     5      $ Телефон��6%   $	 * 6P   
 66   �� *Phone+6P   4  .System.Windows.Controls.DataGridTemplateColumn  5      $ Действия��6)   $		 120 6;      CellTemplate 5      �� 5       .R�5!      ��  �� Orientation$ 
Horizontal=�6)   $��Center=�6B   .+�5"   "   �� -   5        $��Изменить��5"   )   $��60��6<   $��5q�6G   6R   �� 5#   "   -   5        $��Удалить��5#   )   $��60��6;   $��5q�6F   6Q   5$      5%      5'      5)      �� 5+   
   -   5        $#��Создать клиента��5+      $	��150��6+   $��30��67   $��14(�6C   ���#2980B9
�������$
��Left=�5,   9   $��Bottom=�5-      $
��0,10q�6,   6:   �� 5/   
   -   5        $��Вернуться��5/      $	��150��6%   $��30��61   $��14(�6=   ���#E74C3C
�������$��Right=�50   9   $��Bottom=�51      $��Center=�6,   $
��0,10q�6P   6^   52      53      