import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-loader',
  templateUrl: './loader.component.html',
  styleUrls: ['./loader.component.scss']
})
export class LoaderComponent implements OnInit {

  isShown: boolean;

  constructor() { }

  ngOnInit() {
  }

  showLoader() {
    this.isShown = true;
  }

  hideLoader() {
    this.isShown = false;
  }
}
