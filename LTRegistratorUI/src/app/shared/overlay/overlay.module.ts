import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { OverlayService } from './overlay.service';
import { OverlayComponent } from './overlay.component';

@NgModule({
  imports: [
    CommonModule,
    MatSnackBarModule
  ],
  entryComponents: [
    OverlayComponent
  ],
  declarations: [
    OverlayComponent
  ],
  providers: [ OverlayService ]
})
export class OverlayModule { }
