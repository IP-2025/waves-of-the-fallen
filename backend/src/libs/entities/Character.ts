import { Column, Entity, JoinColumn, OneToOne, PrimaryColumn } from 'typeorm';

@Entity()
export class Character {

  @PrimaryColumn({
    type: 'int',
    nullable: false,
    unique: true,
  })
  characterId!: string;

  @Column({
    type: 'varchar',
    nullable: false,
    unique: true,
  })
  name!: string;

  @Column({
    type: 'int',
    nullable: false,
  })
  speed!: string;

  @Column({
    type: 'int',
    nullable: false,
  })
  health!: string;

  @Column({
    type: 'int',
    nullable: false,
  })
  dexterity!: string;

  @Column({
    type: 'int',
    nullable: false,
  })
  intelligence!: string;
}
