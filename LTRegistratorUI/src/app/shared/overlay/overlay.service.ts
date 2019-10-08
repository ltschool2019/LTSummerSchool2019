import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig, MatSnackBarRef } from '@angular/material';
import { OverlayComponent } from './overlay.component';
import { DataOverlayType } from './data-overlay-type.enum';

@Injectable()
export class OverlayService {

  private config: MatSnackBarConfig;

  private _snackBar: MatSnackBarRef<OverlayComponent>;

  constructor(private snackBar: MatSnackBar) {
    this.config = {
      duration: 0,
    };
  }

  info(message: string, action?: string, config?: MatSnackBarConfig, hasBottom?: boolean): MatSnackBarRef<OverlayComponent> {
    config = Object.assign(
      {},
      this.config,
      config || {},
      {
        data: {
          type: DataOverlayType.INFO,
          message: message,
          action: action,
          hasBottom: hasBottom || false
        }
      });
    if (this._snackBar) {
      this._snackBar.dismiss();
    }
    this._snackBar = this.snackBar.openFromComponent(OverlayComponent, config);
    this._snackBar.instance.overlayRef = this._snackBar;
    return this._snackBar;
  }

  success(message: string, action?: string, config?: MatSnackBarConfig, hasBottom?: boolean): MatSnackBarRef<OverlayComponent> {
    config = Object.assign(
      {},
      this.config,
      config || {},
      {
        data: {
          type: DataOverlayType.SUCCESS,
          message: message,
          action: action,
          hasBottom: hasBottom || false
        }
      });
    if (this._snackBar) {
      this._snackBar.dismiss();
    }
    this._snackBar = this.snackBar.openFromComponent(OverlayComponent, config);
    this._snackBar.instance.overlayRef = this._snackBar;
    return this._snackBar;
  }

  danger(message: string, action?: string, config?: MatSnackBarConfig, hasBottom?: boolean): MatSnackBarRef<OverlayComponent> {
    config = Object.assign(
      {},
      this.config,
      config || {},
      {
        data: {
          type: DataOverlayType.DANGER,
          message: message,
          action: action || null,
          hasBottom: hasBottom || false
        }
      });
    if (this._snackBar) {
      this._snackBar.dismiss();
    }
    this._snackBar = this.snackBar.openFromComponent(OverlayComponent, config);
    this._snackBar.instance.overlayRef = this._snackBar;
    return this._snackBar;
  }

  clear() {
    this.snackBar.dismiss();
  }
}
