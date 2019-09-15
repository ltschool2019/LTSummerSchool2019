import { DataOverlayType } from './data-overlay-type.enum';

export interface DataOverlay {
  massage: string;
  message?: string;
  type: DataOverlayType;
  action?: string;
  hasBottom?: boolean;
}
