   M S B A M L   `   `   `  ����I  EK5E Memory Map, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:K5E_Memory_Map.UIModuleK5E_Memory_Map.UIModule  5      )    #K5E_Memory_Map.UIModule.DisplayMenu   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      7local%clr-namespace:K5E_Memory_Map.UIModule     5      .��5	      � 
�������.+�5
   
   �� $	��Top=�6   $��Display Options��6,     �� Padding$  5,0,0,0q�6C   $��20(�6U   �� 5   
   $��1,30,0,0q�6   .+�5      �� -   5        $
��Text��5      $
��Text��6#   $��15(�6Y    d� 	IsChecked 62   �� *IsOption1Checked+6{   �� 5      -   5        $	��Tag��5      $	��Tag��6"   $��15(�6W    60   �� *IsOption2Checked+6x   �� 5      -   5        $	��Mem��5      $	��Mem��6#   $��15(�6X    61   �� *IsOption3Checked+6y   �� 5      -   5        $��Colour��5      $��Colour��6%   $��15(�6]    66   �� *IsOption4Checked+6~   �� 5      -   5        $��Stated��5      $��Stated��6%   $��15(�6]    66   �� *IsOption4Checked+6�   �� 5      $��Memory Length:��6   $��15(�6.   5      �� 5   
    �� Orientation$ 
Horizontal=�6   $��	1,150,0,0q�6.   .+�5      	� -   5        
 � Tag$ 4��5      $��4��6#    	� 	GroupName$ MemLen��6W   $��15(�6j    6/   �� *IsOption1Selected+6x   	� 5      -   5        $��DefSub��5      $ 8��6)   $��8��62   $ MemLen��6f   $��15(�6y    6>   �� *IsOption2Selected+6�   	� 5      -   5        $ 16��5      $��16��6$   $ MemLen��6Y   $��15(�6l    61   �� *IsOption3Selected+6z   	� 5      -	   5        $ 32��5      $��32��6$   $ MemLen��6Y   $��15(�6l    61   �� *IsOption4Selected+6z   5      �� 5   
   $ 
Horizontal=�6   $��	1,175,0,0q�6.   $y�Hidden=�6A   .+�5       �� $��Zoom ��6   $��15(�6%   $��0,-4,0,0q�63   	� 5!      $
 Zoom��6B   $��15(�6S    6   �� *IsOption1Selected+6S   	� 5"      $
 Zoom��6B   $��15(�6S    6   �� *IsOption2Selected+6S   	� 5#      $
 Zoom��6B   $��15(�6S    6   �� *IsOption3Selected+6S   5$      �� 5&   
   $ Vertical=�6   $��	1,170,0,0q�6,   .+�5'      �� -
   5        $��AltGraph��5'      $��	Alt Graph��6'   $��15(�6b    6;   �� *IsOption1Checked+6�   �� 5(      -   5        $��Focus��5(      $"��Auto Focus Current Mem Value��6I   $��15(�6q    6$   �� *IsFocusChecked+6�   �� 5)      -   5        $��Go Current Mem��5)      $��25��6B   $	��145��6O   $��-2,0,0,0q�6\   �� 5*      -   5        $��Go Root��5*      $��25��67   $	��145��6D   $��-2,5,0,0q�6Q   �� 5+      -   5        $��
Reset Zoom��5+      $��25��6=   $	��145��6J   $��-2,5,0,0q�6W   5,      �� 5.   
   -   5        $��PracButt��5.      $ Vertical=�6&   $��	1,317,0,0q�6=   $y�	Collapsed=�6P   .+�5/      �� -   5        $��Selected to Top Good��5/      $��25��65   $	��145��6A   $��-2,5,0,0q�6N   
������6x   �� 50      -   5        $
��Good��50      $��25��6%   $	��145��61   $��-2,5,0,0q�6>   
������6h   �� 51      -   5        $	��Bad��51      $��25��6$   $	��145��60   $��-2,5,0,0q�6=   
���Gc��6c   �� 52      -   5        $��Selected Down Bad��52      $��25��62   $	��145��6>   $��-2,5,0,0q�6K   
���Gc��6q   �� 53      -   5        $��Neutral/Clear��53      $��25��6.   $	��145��6:   $��-2,5,0,0q�6G   
������6t   �� 54      -   5        $��	Clear All��54      $��25��6*   $	��145��66   $��-2,5,0,0q�6C   
������6p   55      57      58      