import { Column, Entity, JoinColumn, OneToOne, PrimaryColumn } from 'typeorm';

@Entity()
export class Settings {
  @PrimaryColumn({
    type: 'varchar',
    nullable: false,
    unique: true,
  })
  player_id!: string;

  @Column({
    type: 'double',
  })
  musicVolume!: string;

  @Column({
    type: 'double',
  })
  soundVolume!: string;

  @OneToOne(() => Settings, (settings) => settings.player_id)
  @JoinColumn()
  settings!: Settings;
}