   M S B A M L   `   `   `  ����I  EK5E Memory Map, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:K5E_Memory_MapK5E_Memory_Map  5   	   A%clr-namespace:K5E_Memory_Map.UIModuleK5E_Memory_Map.UIModule  64       K5E_Memory_Map.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:K5E_Memory_Map     5   	   >userControls%clr-namespace:K5E_Memory_Map.UIModule     64   -   5        $��MMainWindow��5   	     � Title$  SFA Memory Map��5   	   $	��700��6    $
��1017��6-    � WindowState$ Normal=�5	   	    � SizeToContent$ Manual=�5
   	   .��5      �  � ColumnDefinitions 5   
   �� 5      $	��250 �6   �� 5       � RowDefinitions 5   
   �� 5      �� 5      $	��160 �6   .+�5   
   � $��1��6   $��1��6    5      �� 5      $	��211 �6#   �� 5      �� 5      $��0* �6#   �� 5      $��0* �6#   .+�5       �� 
���   �$	��0.5q�6*    �� Padding$ 0q�6@   �� 5"      $��1��6   
���   �$	��0.5q�6;   $ 0q�6Q   (   "K5E_Memory_Map.UIModule.TagButtons  5#      -   5          Name$ TTagButtons��5#   &   � 5&      $��1��6    5'      �� 5(      �� 5)      .+�5+      )   #K5E_Memory_Map.UIModule.NodeDetails  -   5          Name$ NNodeDetails��5+   +   -   'K5E_Memory_Map.UIModule.SelectedDetails  5,      -   5          Name$ SSelectedDetails��5,   /   $��1��6J   �� 5-      $��1��6   
���   �$	��0.1q�6?   $ 0q�6U   �� 5.      $��1��6   
���   �$��1q�6<   $ 0q�6P   5/      50      � 53   
   $��0��6   $��0��6    54      �� 55      $��2* �6#   �� 56      $��3* �6#   .+�58      &    K5E_Memory_Map.UIModule.MainMenu  -   5        	  Name$	 	MMainMenu��58   $   )   #K5E_Memory_Map.UIModule.DisplayMenu  59      -   5        
  Name$
 DDisplayMenu��59   '   $��1��6>   �� 5:      
���   �$	��0.5q�6*   $ 0q�6@   �� 5;      $��2��6   
���   �$	��0.5q�6;   $ 0q�6Q   5=      � 5@   
   $��0��6   $��1��6   .+�5A      �� 
���   �$	��0.5q�6*   $ 0q�6@   (   "K5E_Memory_Map.UIModule.SmallGraph  5B      -   5          Name$ SSmallGraph��5B   &   $y�Hidden=�6<   '   !K5E_Memory_Map.UIModule.FullGraph  5C      -   5          Name$ 
FFullGraph��5C   %   $y�Hidden=�6:   &    K5E_Memory_Map.UIModule.SaveMenu  5D      -	   5          Name$ 	SSaveMenu��5D   $   $y�Visible=�67   &	    K5E_Memory_Map.UIModule.Analysis	  5E      -
   5         	 Name$ 	AAnalysis��5E   $   $y�Hidden=�67   &
    K5E_Memory_Map.UIModule.Practice
  5F      -   5         
 Name$ 	PPractice��5F   $   $y�Hidden=�67   #   K5E_Memory_Map.UIModule.Merge  5G      -   5          Name$ MMerge��5G   !   $y�Hidden=�61   5I      � 5L   
   $��1��6   $��0��6    5M      �� 5N      $��2* �6    �� 5O      �� 5P      .+�5R      -   'K5E_Memory_Map.UIModule.ProcessingState  -   5          Name$ ProcessingState��5R   +   $��2��6D   (   "K5E_Memory_Map.UIModule.MemDisplay  5S      -   5          Name$ MMemDisplay��5S   &   �� 5T      
���   �$	��0.5q�6*   $ 0q�6@   �� 5U      $��1��6   
���   �$	��0.5q�68   $ 0q�6N   �� 5V      -   5        $��TB_Hash��5V      $��1��6(    �� Padding$ 0,2,0,0q�6g   ��65   �� *Hash+ �� StringFormat$ 	Heap: {0}��6g   �� 5W      -   5        $��TB_Frame��5W      $��1��6)   $ 0,19,0,0q�6j   ��66   �� *Frame+$ 
Frame: {0}��6j   �� 5X      $��2��6   
���   �$	��0.5q�68   $ 0q�6N   �� 5Z      -   5        $��PauseButton��5Z      $��40��6)   $��Bottom=�65   $��Right=�6P   $��40��6l   $��0,0,0,0q�6x   $��2��6�    K� ZIndex$ 1��6�   .��5[      � .+�5\      �� $��Right=�6    $��8��6<   $��20��6G   $��	-13,0,0,0q�6S   $��2��6f   $ 2��6s   
�������6�   �� 5]      $��Right=�6    $��8��6<   $��20��6G   $��0,0,13,0q�6S   $��2��6e   $ 2��6r   
�������6�   5^      5_      �� 5a      -   5        $��
PlayButton��5a      $��40��6(   $��Bottom=�64   $��Right=�6O   $��40��6k   $��0,0,0,0q�6w   $��2��6�   $ 1��6�   $y�Hidden=�6�   .��5b      � .+�5d      F� "���� � ��� � ?� -1� � ���	
���   �$��	-8,-8,0,0q�6F   $��Center=�6Z   $��Center=�6w   5e      5f      5h      5j      5k      