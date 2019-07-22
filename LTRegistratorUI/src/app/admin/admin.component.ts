import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {

  ngOnInit() {
  }

  selectedAll: any;
  checkboxes: any;
  constructor() {
    this.checkboxes = [
      { name: '', selected: false },
      { name: '', selected: false },
      { name: '', selected: false },
    ]
  }
  selectAll() {
    for (var i = 0; i < this.checkboxes.length; i++) {
      this.checkboxes[i].selected = this.selectedAll;
    }
  }
  checkIfAllSelected() {
    this.selectedAll = this.checkboxes.every(function (item: any) {
      return item.selected == true;
    })
  }

}
