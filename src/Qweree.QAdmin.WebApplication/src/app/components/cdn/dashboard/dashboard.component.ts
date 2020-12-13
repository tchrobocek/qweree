import {Component, OnInit, SimpleChanges} from '@angular/core';
import {ExplorerDirectory, ExplorerFile} from '../../../model/cdn/ExplorerObject';
import {CdnAdapterService} from '../../../services/cdn/cdn-adapter.service';

@Component({
  selector: 'app-cdn-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  public totalCount = 0;
  public totalSize = 0;

  constructor(
    private cdnAdapter: CdnAdapterService
  ) { }

  ngOnInit(): void {
    this.cdnAdapter.explore('/')
      .subscribe(objects => {
        objects.forEach(o => {
          const dir = (o as ExplorerDirectory);
          if (dir.totalCount) {
            this.totalSize += dir.totalSize;
            this.totalCount += dir.totalCount;
          }
          const file = (o as ExplorerFile);
          if (file.id) {
            this.totalSize += file.size;
            this.totalCount++;
          }
        });
      });
  }
}
