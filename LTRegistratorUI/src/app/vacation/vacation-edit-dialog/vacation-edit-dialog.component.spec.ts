import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VacationEditDialogComponent } from './vacation-edit-dialog.component';

describe('VacationEditDialogComponent', () => {
  let component: VacationEditDialogComponent;
  let fixture: ComponentFixture<VacationEditDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VacationEditDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VacationEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
