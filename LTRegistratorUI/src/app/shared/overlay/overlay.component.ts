import { Component, Inject, OnInit } from '@angular/core';
import { MAT_SNACK_BAR_DATA, MatSnackBarRef } from '@angular/material';
import { DataOverlay } from './overlay.interface';
import { DataOverlayType } from './data-overlay-type.enum';

@Component({
  selector: 'overlay',
  templateUrl: './overlay.component.html',
  styleUrls: ['./overlay.component.scss'],
})
export class OverlayComponent implements OnInit {
  public DATA_OVERLAY_TYPE = DataOverlayType;
  public overlayRef: MatSnackBarRef<OverlayComponent>;
  constructor(
    @Inject(MAT_SNACK_BAR_DATA) public data: DataOverlay
  ) {}

  ngOnInit() {}

  close() {
    this.overlayRef.closeWithAction();
  }
}
