   M S B A M L   `   `   `  ����O  KDemoButlerController, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:DemoButler
DemoButler  5   	       DemoButler.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   )convclr-namespace:DemoButler     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  
DemoButler��5   	   $	��320��6   $	��550��6)    � WindowState$ 	Minimized=�65    � Icon$ Remote+ControlGR.ico��6M    � 	Resources 5      #   DemoButler.ListValueConverter   conv��%   &	          5   
   61   5      .��5	   
   �  � RowDefinitions 5
   
   �� 5      5      �� 5      5      �� 5      $��40 �6   6*   .+�5   
   �� -   5         ��Name$
 show��5   $   
����  �$��3��62   $��24(�6F   ��6T   ��  �� Path$ ShowContent�.��6v   5      �� 5   
   -   5         ��Name$ actions��5      $��1��6   $��3��60   ��6D   �� $ Actions���5      �� 5      .R�5      ��  �� Orientation$ 
Horizontal=�6!   .+�5      �� 
���  ��$��30��66   ��6A   �� $
 Type�6^   �� 5      
����  ���67   �� $ Content�	 �� 	Converter#	 [  6�   5      5      5      � 5   
   $��2��6   
 � ColumnDefinitions
 5       �� 5!      6%   �� 5"      6%   �� 5#      6%   �� 5$      6%   �� 5%      6%   .+�5'      �� -   5         ��Name$
 Init��5'   3   $��2��6   $��0��6#   .��6W   Init6]   �� 5(      -   5        $ Previous��5(   3   $��2��6   $��1��6#   .��6_   
Previous6i   �� 5)      -   5        $
 Next��5)   3   $��2��6   $��2��6#   .��6W   Next6]   �� 5*      -   5        $
 Skip��5*   3   $��2��6   $��3��6#   .��6W   Skip6]   �� 5+      -   5        $ Restart��5+   3   $��2��6   $��4��6#   .��6]   	Restart6f   5,      5.      5/      