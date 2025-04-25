import { Column, Entity, JoinColumn, OneToOne, PrimaryColumn } from 'typeorm';

@Entity()
export class UnlockedCharacter {

  @Column({
    type: 'varchar',
    nullable: false,
    unique: true,
  })
  player_id!: string;

  @Column({
    type: 'varchar',
    nullable: false,
    unique: true,
  })
  characterId!: string;

  @Column({
    type: 'int',
    nullable: false,
  })
  level!: number;

}